using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using LibTessDotNet;
using Unity.Collections;
using Mesh = UnityEngine.Mesh;
using Clipper2Lib;
using ViJApps.CanvasTexture.ThirdParty;

namespace ViJApps.CanvasTexture.Utils
{
    public enum LineEndingType
    {
        Joined,
        Butt,
        Square,
        Round
    }

    public enum LineJoinType
    {
        Square,
        Round,
        Miter
    };

    public struct VertexDataPositionUV
    {
        public float3 Pos;
        public float2 TextCoord;

        public VertexDataPositionUV(float3 pos, float2 textCoord)
        {
            Pos = pos;
            TextCoord = textCoord;
        }
    }

    public static class MeshTools
    {
        private static readonly Tess Tess = new Tess(new NullPool()); //TODO: Implement JobSystem Version
        private static readonly Clipper64 Clipper = new();
        private static readonly ClipperOffset ClipperOffset = new();

        #region Lines, polygons

        /// <summary>
        /// Creates polyline that can be opened or closed
        /// </summary>
        /// <param name="points">points of the polyline</param>
        /// <param name="thickness">line thickness</param>
        /// <param name="endType">line ending type: round, butt, square, joined (closed spline)</param>
        /// <param name="joinType">connection type between segments: round, butt, square, miter</param>
        /// <param name="miterLimit">the ratio between line thickness and </param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh CreatePolyLine(
            List<float2> points,
            float thickness,
            LineEndingType endType = LineEndingType.Butt,
            LineJoinType joinType = LineJoinType.Miter,
            float miterLimit = 0f,
            Mesh mesh = null)
        {
            mesh = CreateMeshOrClear(ref mesh);
            miterLimit = math.max(0, miterLimit);

            //CLIPPER CANNOT CREATE SIGNED OFFSETS FOR OPEN LINES, TODO: IMPLEMENT IT
            ClipperOffset.Clear();
            var initialPath = Converters.Float2ToPath64(points);
            var offsetPath = new Paths64();
            ClipperOffset.AddPath(initialPath, (JoinType)joinType, endType.GetLineEndType());
            ClipperOffset.MiterLimit = miterLimit;
            offsetPath.AddRange(ClipperOffset.Execute(Converters.DoubleToClipper(thickness)));

            //now lets make all inside filled
            Clipper.Clear();
            Clipper.AddSubject(offsetPath);
            var result = new Paths64();
            Clipper.Execute(ClipType.Union, FillRule.EvenOdd, result);

            mesh = CreateMeshFromClipper(result, mesh);
            return mesh;
        }

        public static (Mesh polygonMesh, Mesh lineMesh) CreatePolygon(
            List<float2> solidPolygon,
            float lineThickness,
            float lineOffset = 0.5f,
            LineJoinType joinType = LineJoinType.Miter,
            float miterLimit = 0f,
            Mesh polygonMesh = null,
            Mesh lineMesh = null) =>
            CreatePolygon(new List<List<float2>>() { solidPolygon }, lineThickness, lineOffset, joinType, miterLimit, polygonMesh, lineMesh);


        public static (Mesh polygonMesh, Mesh lineMesh) CreatePolygon(
            List<List<float2>> solidPolygons,
            float lineThickness,
            float lineOffset = 0.5f,
            LineJoinType joinType = LineJoinType.Miter,
            float miterLimit = 0f,
            Mesh polygonMesh = null,
            Mesh lineMesh = null) =>
            CreatePolygon(solidPolygons, null, lineThickness, lineOffset, joinType, miterLimit, polygonMesh, lineMesh);

        public static (Mesh polygonMesh, Mesh lineMesh) CreatePolygon(
            List<float2> solidPolygon,
            List<float2> holePolygon,
            float lineThickness,
            float lineOffset = 0.5f,
            LineJoinType joinType = LineJoinType.Miter,
            float miterLimit = 0f,
            Mesh polygonMesh = null,
            Mesh lineMesh = null) => CreatePolygon(new List<List<float2>>() { solidPolygon }, new List<List<float2>>() { holePolygon }, lineThickness, lineOffset, joinType, miterLimit, polygonMesh,
            lineMesh);

        public static (Mesh polygonMesh, Mesh lineMesh) CreatePolygon(
            List<List<float2>> solidPolygons,
            List<List<float2>> holePolygons,
            float lineThickness,
            float lineOffset = 0.5f,
            LineJoinType joinType = LineJoinType.Miter,
            float miterLimit = 0f,
            Mesh polygonMesh = null,
            Mesh lineMesh = null)
        {
            miterLimit = math.max(0f, miterLimit);
            lineThickness = math.max(0f, lineThickness);
            lineOffset = math.clamp(lineOffset, 0f, 1f);

            var solidResult = UnionContoursToPaths64(solidPolygons);
            var holeResult = UnionContoursToPaths64(holePolygons);

            //Now subtract them. Result is a polygon with holes
            var initialPoly = Subtract(solidResult, holeResult);

            //Now we create offsets to render lines
            var positiveResult =
                OffsetPolygons(initialPoly, (JoinType)joinType, lineThickness * lineOffset, miterLimit);
            var negativeResult = OffsetPolygons(initialPoly, (JoinType)joinType, -lineThickness * (1f - lineOffset),
                miterLimit);

            var linePoly = Subtract(positiveResult, negativeResult);

            polygonMesh = CreateMeshFromClipper(negativeResult, polygonMesh);
            lineMesh = CreateMeshFromClipper(linePoly, lineMesh);

            return (polygonMesh, lineMesh);
        }

        public static Mesh CreateMeshFromContourPolygons(List<List<float2>> contours, Mesh mesh = null, WindingRule windingRule = WindingRule.EvenOdd)
        {
            foreach (var contour in contours)
                Tess.AddContour(contour.ToContourVertices());
            Tess.Tessellate(windingRule, ElementType.Polygons, polySize: 3, combineCallback: null,
                normal: new Vec3(0, 0, -1));
            mesh = Tess.TessToUnityMesh(mesh);
            return mesh;
        }

        /// <summary>
        /// Creates mesh for simple line
        /// </summary>
        /// <param name="fromCoord"></param>
        /// <param name="toCoord"></param>
        /// <param name="aspectMatrix"></param>
        /// <param name="width"></param>
        /// <param name="extendStartEnd"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh CreateLine(float2 fromCoord, float2 toCoord, float3x3 aspectMatrix, float width,
            bool extendStartEnd = false, Mesh mesh = null)
        {
            mesh = CreateMeshOrClear(ref mesh);
            var direction = toCoord - fromCoord;

            var aspectDir = math.normalize(direction.InverseTransformDirection(aspectMatrix)) * width * 0.5f;
            var dir = aspectDir.RotateVectorCwHalfPi().TransformDirection(aspectMatrix);

            var extend = extendStartEnd ? aspectDir.TransformDirection(aspectMatrix) : float2.zero;
            var texSpaceFrom = (fromCoord - extend);
            var texSpaceTo = (toCoord + extend);

            var p0 = texSpaceFrom - dir;
            var p1 = texSpaceTo - dir;
            var p2 = texSpaceTo + dir;
            var p3 = texSpaceFrom + dir;

            mesh = CreateMeshFromFourPoints(p0, p1, p2, p3, mesh);
            return mesh;
        }

        #endregion

        public static Mesh CreateRect(float2 centerCoord, float2 size, float3x3 aspectMatrix, Mesh mesh = null)
        {
            mesh = CreateMeshOrClear(ref mesh);
            var aspectSize = size.TransformDirection(aspectMatrix);

            var halfSize = aspectSize / 2;
            var p0 = centerCoord + new float2(-halfSize.x, -halfSize.y);
            var p1 = centerCoord + new float2(-halfSize.x, +halfSize.y);
            var p2 = centerCoord + new float2(+halfSize.x, +halfSize.y);
            var p3 = centerCoord + new float2(+halfSize.x, -halfSize.y);

            mesh = CreateMeshFromFourPoints(p0, p1, p2, p3, mesh);
            return mesh;
        }

        public static Mesh CreateRectTransformed(float2 size, float3x3 transform, Mesh mesh = null)
        {
            mesh = CreateMeshOrClear(ref mesh);
            var halfSize = size / 2;

            var p0 = new float2(-halfSize.x, -halfSize.y).TransformPoint(transform);
            var p1 = new float2(-halfSize.x, +halfSize.y).TransformPoint(transform);
            var p2 = new float2(+halfSize.x, +halfSize.y).TransformPoint(transform);
            var p3 = new float2(+halfSize.x, -halfSize.y).TransformPoint(transform);

            mesh = CreateMeshFromFourPoints(p0, p1, p2, p3, mesh);
            return mesh;
        }

        public static List<List<float2>> TransformPoints(this List<List<float2>> pointLists, float3x3 matrix)
        {
            var result = new List<List<float2>>(pointLists.Count);
            foreach (var pointsList in pointLists)
                result.Add(pointsList.TransformPoints(matrix));
            return result;
        }

        public static List<float2> TransformPoints(this List<float2> points, float3x3 matrix)
        {
            var result = new List<float2>(points.Count);
            foreach (var point in points)
                result.Add(point.TransformPoint(matrix));
            return result;
        }

        /// <summary>
        /// Creates convex mesh polygon from 3 points, 0 1 2 and 0 2 3
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static Mesh CreateMeshFromFourPoints(float2 p0, float2 p1, float2 p2, float2 p3, Mesh mesh = null)
        {
            CreateMeshOrClear(ref mesh);
            var meshDataArr = Mesh.AllocateWritableMeshData(1);
            var meshData = meshDataArr[0];
            meshData.SetVertexBufferParams(4,
                new VertexAttributeDescriptor(VertexAttribute.Position, dimension: 3, stream: 0),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2, stream: 0));
            meshData.SetIndexBufferParams(6, IndexFormat.UInt16);

            var vertices = meshData.GetVertexData<VertexDataPositionUV>();
            var indices = meshData.GetIndexData<ushort>();
            vertices[0] = new VertexDataPositionUV(p0.ToFloat3(), new float2(0f, 0f));
            vertices[1] = new VertexDataPositionUV(p1.ToFloat3(), new float2(0f, 1f));
            vertices[2] = new VertexDataPositionUV(p2.ToFloat3(), new float2(1f, 1f));
            vertices[3] = new VertexDataPositionUV(p3.ToFloat3(), new float2(1f, 0f));

            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;

            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;

            var subMesh = new SubMeshDescriptor()
            {
                vertexCount = 4,
                indexCount = 6,
                topology = MeshTopology.Triangles,
            };
            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, subMesh);
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArr, mesh);

            return mesh;
        }

        private static Mesh CreateMeshOrClear(ref Mesh mesh)
        {
            if (mesh == null)
                mesh = new Mesh();
            else
                mesh.Clear();
            return mesh;
        }

        #region Clipper operations for mesh

        /// <summary>
        /// Boolean 2d operation, takes operandsA and subtracts operandsB from it
        /// </summary>
        /// <param name="operandA"></param>
        /// <param name="operandB"></param>
        /// <returns></returns>
        private static Paths64 Subtract(Paths64 operandA, Paths64 operandB)
        {
            var result = new Paths64();
            Clipper.Clear();
            Clipper.AddSubject(operandA);
            Clipper.AddClip(operandB);
            Clipper.Execute(ClipType.Difference, FillRule.EvenOdd, result);
            return result;
        }

        /// <summary>
        /// Boolean operation, takes all contours and unions them
        /// </summary>
        /// <param name="contours"></param>
        /// <returns></returns>
        private static Paths64 UnionContoursToPaths64(List<List<float2>> contours)
        {
            if (contours == null || contours.Count == 0)
                return new Paths64();

            Clipper.Clear();
            foreach (var polygon in contours)
            {
                var initialPath = Converters.Float2ToPath64(polygon);
                Clipper.AddSubject(initialPath);
            }

            var result = new Paths64();
            Clipper.Execute(ClipType.Union, FillRule.EvenOdd, result);
            return result;
        }

        /// <summary>
        /// Takes paths and offserts them
        /// </summary>
        /// <param name="polygons"></param>
        /// <param name="joinType"></param>
        /// <param name="offset"></param>
        /// <param name="miterLimit"></param>
        /// <returns></returns>
        private static Paths64 OffsetPolygons(Paths64 polygons, JoinType joinType, float offset, float miterLimit)
        {
            //TODO: think about allocations of paths64
            ClipperOffset.Clear();
            ClipperOffset.AddPaths(polygons, (JoinType)joinType, EndType.Polygon);
            ClipperOffset.MiterLimit = miterLimit;
            var offsetResult = new Paths64(ClipperOffset.Execute(Converters.DoubleToClipper(offset)));
            return offsetResult;
        }

        /// <summary>
        /// Takes Clipper paths64 and converts them to Unity mesh
        /// </summary>
        /// <param name="contours">the contours to convert</param>
        /// <param name="mesh">the mesh to be filled. the new one is created when null passed</param>
        /// <returns></returns>
        private static Mesh CreateMeshFromClipper(Paths64 contours, Mesh mesh = null)
        {
            foreach (var contour in contours)
                Tess.AddContour(Converters.Path64ToContourVertexArr(contour));
            Tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3, combineCallback: null,
                normal: new Vec3(0, 0, -1));
            mesh = Tess.TessToUnityMesh(mesh);
            return mesh;
        }

        #endregion

        #region Converters

        private static Mesh TessToUnityMesh(this Tess tess, Mesh mesh)
        {
            var (positions, indices) = tess.ToPositionsAndUshortIndicesNativeArrays();
            mesh = CreateMeshFromNativeArrays(positions, indices, mesh);
            positions.Dispose();
            indices.Dispose();
            return mesh;
        }

        private static Mesh CreateMeshFromNativeArrays(NativeArray<float3> positions, NativeArray<ushort> indices, Mesh mesh = null)
        {
            mesh = CreateMeshOrClear(ref mesh);

            var meshDataArr = Mesh.AllocateWritableMeshData(1);
            var meshData = meshDataArr[0];
            meshData.SetVertexBufferParams(positions.Length,
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3));
            meshData.SetIndexBufferParams(indices.Length, IndexFormat.UInt16);

            meshData.GetVertexData<float3>().CopyFrom(positions);
            meshData.GetIndexData<ushort>().CopyFrom(indices);

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indices.Length));
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArr, mesh, MeshUpdateFlags.Default);
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.RecalculateNormals();
            return mesh;
        }

        #endregion
    }
}