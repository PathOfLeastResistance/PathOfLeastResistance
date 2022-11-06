using System;
using System.Collections.Generic;
using System.Linq;
using Clipper2Lib;
using LibTessDotNet;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using ViJApps.CanvasTexture.Utils;

namespace ViJApps.CanvasTexture.ThirdParty
{
    /// <summary>
    /// Class helper for conversion from tess data to unity data
    /// </summary>
    public static class Converters
    {
        /// <summary>
        /// Converts List of float2 to array of ContourVertex
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static ContourVertex[] ToContourVertices(this List<float2> vertices)
        {
            var result = new ContourVertex[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
                result[i] = new ContourVertex(vertices[i].ToVec3());
            return result;
        }

        /// <summary>
        /// Converts array of float2 to array of ContourVertex
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public static ContourVertex[] ToContourVertices(this float2[] vertices)
        {
            var result = new ContourVertex[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                result[i] = new ContourVertex(vertices[i].ToVec3());
            return result;
        }

        /// <summary>
        /// Converts tess result to native arrays of vertices and indices
        /// </summary>
        /// <param name="tess"></param>
        /// <returns></returns>
        public static (NativeArray<float3> positions, NativeArray<ushort> indices) ToPositionsAndUshortIndicesNativeArrays(this Tess tess)
        {
            if (tess.Vertices == null || tess.Elements == null)
                return new(new NativeArray<float3>(0, Allocator.Temp), new NativeArray<ushort>(0, Allocator.Temp));

            var positions = new NativeArray<float3>(tess.Vertices.Length, Allocator.Temp);
            for (int i = 0; i < tess.Vertices.Length; i++)
                positions[i] = tess.Vertices[i].Position.ToFloat3();

            var indices = new NativeArray<ushort>(tess.Elements.Length, Allocator.Temp);
            for (int i = 0; i < tess.Elements.Length; i++)
                indices[i] = (ushort)tess.Elements[i];
            return (positions, indices);
        }

        /// <summary>
        /// Converts tess result to native arrays of vertices and indices
        /// </summary>
        /// <param name="tess"></param>
        /// <returns></returns>
        public static (NativeArray<float3> positions, NativeArray<int> indices) ToPositionsAndIntIndicesNativeArrays(this Tess tess)
        {
            var positions = new NativeArray<float3>(tess.Vertices.Length, Allocator.Persistent);
            var indices = new NativeArray<int>(tess.Elements.Length, Allocator.Persistent);
            for (int i = 0; i < tess.Vertices.Length; i++)
                positions[i] = tess.Vertices[i].Position.ToFloat3();
            for (int i = 0; i < tess.Elements.Length; i++)
                indices[i] = tess.Elements[i];
            return (positions, indices);
        }

        /// <summary>
        /// From Vec3 to float2
        /// </summary>
        /// <param name="vec3"></param>
        /// <returns></returns>
        public static float2 ToFloat2(this Vec3 vec3) => new float2(vec3.X, vec3.Y);

        /// <summary>
        /// From Vec3 to float3
        /// </summary>
        /// <param name="vec3"></param>
        /// <returns></returns>
        public static float3 ToFloat3(this Vec3 vec3) => new float3(vec3.X, vec3.Y, vec3.Z);

        /// <summary>
        /// From float2 to Vec3
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vec3 ToVec3(this float2 vertex, float z = 0) => new Vec3(vertex.x, vertex.y, z);

        /// <summary>
        /// From float3 to Vec3
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static Vec3 ToVec3(this float3 vertex) => new Vec3(vertex.x, vertex.y, vertex.z);

        #region Clipper Converters

        private const double DefaultMult = 1e4;

        public static double2 Point64ToDouble2(Point64 clipperPoint, double mult = DefaultMult) =>
            new double2(clipperPoint.X / mult, clipperPoint.Y / mult);

        public static Point64 Double2ToPoint64(double2 doublePoint, double mult = DefaultMult) =>
            new Point64((long)(doublePoint.x * mult), (long)(doublePoint.y * mult));

        public static double ClipperToDouble(double clipperValue, double mult = DefaultMult) => clipperValue / mult;

        public static long DoubleToClipper(double doubleValue, double mult = DefaultMult) => (long)(doubleValue * mult);

        public static EndType GetLineEndType(this LineEndingType lineEndingType)
        {
            return lineEndingType switch
            {
                LineEndingType.Butt => EndType.Butt,
                LineEndingType.Square => EndType.Square,
                LineEndingType.Round => EndType.Round,
                LineEndingType.Joined => EndType.Joined,
                _ => throw new ArgumentException("Unknown line ending type")
            };
        }

        ////////////////////Converters to clipper////////////////////

        /// <summary>
        /// Vector3 IEnumerable to Path64 converter
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="mult"></param>
        /// <returns></returns>
        public static Path64 Vector3ToPoint64(IEnumerable<Vector3> positions, double mult = DefaultMult)
            => new Path64(positions.Select(c => new Point64(c.x * mult, c.y * mult)));

        /// <summary>
        /// Float3 IEnumerable to Path64 converter
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="mult"></param>
        /// <returns></returns>
        public static Path64 Float3ToPoint64(IEnumerable<float3> positions, double mult = DefaultMult)
            => new Path64(positions.Select(c => new Point64(c.x * mult, c.y * mult)));

        /// <summary>
        /// Vector2 IEnumerable to Path64 converter
        /// </summary>
        /// <param name="position"></param>
        /// <param name="mult"></param>
        /// <returns></returns>
        public static Path64 Vector2ToPath64(IEnumerable<Vector2> position, double mult = DefaultMult)
            => new Path64(position.Select(c => new Point64(c.x * mult, c.y * mult)));

        /// <summary>
        /// Float2 IEnumerable to Path64 converter
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="mult"></param>
        /// <returns></returns>
        public static Path64 Float2ToPath64(IEnumerable<float2> positions, double mult = DefaultMult)
            => new Path64(positions.Select(c => new Point64(c.x * mult, c.y * mult)));

        /// <summary>
        /// Float2 transformed with transformationMatrix IEnumerable to Path64 converter 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="transformationMatrix"></param>
        /// <param name="mult"></param>
        /// <returns></returns>
        public static Path64 Float2ToPath64(IEnumerable<float2> positions, float3x3 transformationMatrix,
            double mult = DefaultMult)
        {
            return new Path64(positions.Select(position =>
            {
                position = position.TransformPoint(transformationMatrix);
                return new Point64(position.x * mult, position.y * mult);
            }));
        }

        /// <summary>
        /// ContourVertex array to Path64 converter
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="mult"></param>
        /// <returns></returns>
        public static Path64 ContourVertexArrToPath64(this ContourVertex[] vertices, double mult = DefaultMult)
            => new Path64(vertices.Select(c => new Point64(c.Position.X * mult, c.Position.Y * mult)));

        ////////////////////Converters From Clipper////////////////////

        public static Vector2[] Path64ToVector2Arr(Path64 positions, double mult = DefaultMult)
        {
            var result = new Vector2[positions.Count];
            for (int i = 0; i < positions.Count; i++)
                result[i] = new Vector2((float)(positions[i].X / mult), (float)(positions[i].Y / mult));
            return result;
        }

        public static float2[] Path64ToFloat2Arr(Path64 positions, double mult = DefaultMult)
        {
            var result = new float2[positions.Count];
            for (int i = 0; i < positions.Count; i++)
                result[i] = new float2((float)(positions[i].X / mult), (float)(positions[i].Y / mult));
            return result;
        }

        public static Vector3[] Path64ToVector3Arr(Path64 positions, double mult = DefaultMult)
        {
            var result = new Vector3[positions.Count];
            for (int i = 0; i < positions.Count; i++)
                result[i] = new Vector3((float)(positions[i].X / mult), (float)(positions[i].Y / mult));
            return result;
        }

        public static float3[] Path64ToFloat3Arr(Path64 positions, double mult = DefaultMult)
        {
            var result = new float3[positions.Count];
            for (int i = 0; i < positions.Count; i++)
                result[i] = new float3((float)(positions[i].X / mult), (float)(positions[i].Y / mult), 0f);
            return result;
        }

        public static ContourVertex[] Path64ToContourVertexArr(Path64 positions, double mult = DefaultMult)
        {
            var result = new ContourVertex[positions.Count];
            for (int i = 0; i < positions.Count; i++)
                result[i] = new ContourVertex(new Vec3((float)(positions[i].X / mult), (float)(positions[i].Y / mult),
                    0f));
            return result;
        }

        #endregion
    }
}