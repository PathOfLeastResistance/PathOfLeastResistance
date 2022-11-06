using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ViJApps.CanvasTexture.Utils;

namespace ViJApps.CanvasTexture
{
    public partial class CanvasTexture
    {
        /// <summary>
        /// Draws a polyline (tess and clipper version)
        /// </summary>
        /// <param name="points"></param>
        /// <param name="thickness"></param>
        /// <param name="color"></param>
        /// <param name="endType"></param>
        /// <param name="joinType"></param>
        /// <param name="miterLimit"></param>
        public void DrawPolyLine(
            List<float2> points,
            float thickness,
            Color color,
            LineEndingType endType = LineEndingType.Butt,
            LineJoinType joinType = LineJoinType.Miter,
            float miterLimit = 0f)
        {
            var (mesh, propertyBlock) = AllocateMeshAndPropertyBlock();
            var pointsTransformed = points.TransformPoints(AspectSettings.InverseAspectMatrix2d);
            MeshTools.CreatePolyLine(pointsTransformed, thickness, endType, joinType, miterLimit, mesh);
            
            var material = MaterialProvider.GetMaterial(MaterialProvider.SimpleUnlitTransparentShaderId);
            propertyBlock.SetColor(MaterialProvider.ColorPropertyId, color);
            m_cmd.DrawMesh(mesh, AspectSettings.AspectMatrix3d, material, 0, 0, propertyBlock);
        }

        /// <summary>
        /// Draws complex polygon with strokes (Tess and Clipper version)
        /// </summary>
        /// <param name="solidPolygons">polygons that will be combined</param>
        /// <param name="holePolygons">polygons that will be subtracted</param>
        /// <param name="strokeThickness"></param>
        /// <param name="fillColor"></param>
        /// <param name="strokeColor"></param>
        /// <param name="strokeOffset"></param>
        /// <param name="joinType"></param>
        /// <param name="miterLimit">the ratio relative to line width to cutoff long sharp edges. used only for miter joint types</param>
        public void DrawComplexPolygon(
            List<List<float2>> solidPolygons,
            List<List<float2>> holePolygons,
            float strokeThickness,
            Color fillColor,
            Color strokeColor,
            float strokeOffset = 0.5f,
            LineJoinType joinType = LineJoinType.Miter,
            float miterLimit = 0f)
        {
            var (fillMesh, fillBlock) = AllocateMeshAndPropertyBlock();
            var (lineMesh, lineBlock) = AllocateMeshAndPropertyBlock();

            // Transform points with aspect matrix
            var solidTransformed = solidPolygons.TransformPoints(AspectSettings.InverseAspectMatrix2d);
            var holesTransformed = holePolygons.TransformPoints(AspectSettings.InverseAspectMatrix2d);

            MeshTools.CreatePolygon(solidTransformed, holesTransformed, strokeThickness, strokeOffset, joinType, miterLimit, fillMesh, lineMesh);

            var fillMaterial = MaterialProvider.GetMaterial(MaterialProvider.SimpleUnlitTransparentShaderId);
            var lineMaterial = MaterialProvider.GetMaterial(MaterialProvider.SimpleUnlitTransparentShaderId);

            fillBlock.SetColor(MaterialProvider.ColorPropertyId, fillColor);
            lineBlock.SetColor(MaterialProvider.ColorPropertyId, strokeColor);

            m_cmd.DrawMesh(fillMesh, AspectSettings.AspectMatrix3d, fillMaterial, 0, 0, fillBlock);
            m_cmd.DrawMesh(lineMesh, AspectSettings.AspectMatrix3d, lineMaterial, 0, 0, lineBlock);
        }

        /// <summary>
        /// Draw complex polygon with stroke (Tess and Clipper version)
        /// </summary>
        /// <param name="solidPolygons"></param>
        /// <param name="lineThickness"></param>
        /// <param name="fillColor"></param>
        /// <param name="lineColor"></param>
        /// <param name="lineOffset"></param>
        /// <param name="joinType"></param>
        /// <param name="miterLimit"></param>
        public void DrawComplexPolygon(
            List<List<float2>> solidPolygons,
            float lineThickness,
            Color fillColor,
            Color lineColor,
            float lineOffset = 0.5f,
            LineJoinType joinType = LineJoinType.Miter,
            float miterLimit = 0f)
            => DrawComplexPolygon(
                solidPolygons,
                new List<List<float2>>(),
                lineThickness,
                fillColor,
                lineColor,
                lineOffset,
                joinType,
                miterLimit);

        /// <summary>
        /// Draws simple polygon without strokes (Tess will be used)
        /// </summary>
        /// <param name="contours"></param>
        /// <param name="color"></param>
        public void DrawSimplePolygon(List<List<float2>> contours, Color color)
        {
            var (mesh, propertyBlock) = AllocateMeshAndPropertyBlock();
            mesh = MeshTools.CreateMeshFromContourPolygons(contours, mesh);

            var material = MaterialProvider.GetMaterial(MaterialProvider.SimpleUnlitShaderId);
            propertyBlock.SetColor(MaterialProvider.ColorPropertyId, color);
            m_cmd.DrawMesh(mesh, Matrix4x4.identity, material, 0, 0, propertyBlock);
        }

        /// <summary>
        /// Draws line in pixel coords
        /// </summary>
        /// <param name="pixelFromCoord"></param>
        /// <param name="pixelToCoord"></param>
        /// <param name="pixelThickness"></param>
        /// <param name="color"></param>
        /// <param name="endingStyle"></param>
        public void DrawLinePixels(float2 pixelFromCoord, float2 pixelToCoord, float pixelThickness, Color color, SimpleLineEndingStyle endingStyle = SimpleLineEndingStyle.None)
        {
            var texFromCoord = pixelFromCoord.TransformPoint(m_textureCoordSystem.WorldToZeroOne2d);
            var texToCoord = pixelToCoord.TransformPoint(m_textureCoordSystem.WorldToZeroOne2d);
            var thickness = pixelThickness / m_textureCoordSystem.Height;

            DrawLinePercent(texFromCoord, texToCoord, thickness, color, endingStyle);
        }

        /// <summary>
        /// Draw line in percent coords
        /// </summary>
        /// <param name="percentFromCoord"></param>
        /// <param name="percentToCoord"></param>
        /// <param name="percentHeightThickness"></param>
        /// <param name="color"></param>
        /// <param name="endingStyle"></param>  //TODO: add Square/round/butt endings
        /// <exception cref="Exception"></exception>
        public void DrawLinePercent(float2 percentFromCoord, float2 percentToCoord, float percentHeightThickness, Color color, SimpleLineEndingStyle endingStyle = SimpleLineEndingStyle.None)
        {
            var (lineMesh, propertyBlock) = AllocateMeshAndPropertyBlock();

            propertyBlock.SetColor(MaterialProvider.ColorPropertyId, color);
            Material lineMaterial;
            switch (endingStyle)
            {
                case SimpleLineEndingStyle.None:
                    lineMesh = MeshTools.CreateLine(percentFromCoord, percentToCoord, AspectSettings.AspectMatrix2d,
                        percentHeightThickness, false,
                        lineMesh);
                    lineMaterial =
                        MaterialProvider.GetMaterial(MaterialProvider.SimpleUnlitShaderId);
                    break;
                case SimpleLineEndingStyle.Round:
                    lineMesh = MeshTools.CreateLine(percentFromCoord, percentToCoord, AspectSettings.AspectMatrix2d,
                        percentHeightThickness, true,
                        lineMesh);
                    lineMaterial =
                        MaterialProvider.GetMaterial(MaterialProvider.SimpleLineUnlitShaderId);
                    propertyBlock.SetFloat(MaterialProvider.AspectPropertyId, AspectSettings.Aspect);
                    propertyBlock.SetFloat(MaterialProvider.ThicknessPropertyId, percentHeightThickness);
                    propertyBlock.SetVector(MaterialProvider.FromToCoordPropertyId,
                        new Vector4(percentFromCoord.x, percentFromCoord.y, percentToCoord.x, percentToCoord.y));
                    break;
                default:
                    throw new Exception("Unknown line ending style");
            }

            m_cmd.DrawMesh(lineMesh, Matrix4x4.identity, lineMaterial, 0, -1, propertyBlock);
        }
    }
}