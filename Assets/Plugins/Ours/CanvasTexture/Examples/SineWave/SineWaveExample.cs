// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Unity.Mathematics;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using ViJApps.CanvasTexture;
// using ViJApps.CanvasTexture.Utils;
//
// namespace ViJApps.CanvasTexture.Examples
// {
//     public class SineWaveExample : MonoBehaviour
//     {
//         //Texture Settings
//         [SerializeField] private Renderer m_testRenderer;
//         [SerializeField] private int m_width = 256;
//         [SerializeField] private int m_height = 256;
//
//         //interpretation of Image Aspect (influences on strokes and basic figures, like circles and rectangles, that respect aspect ratio)
//         [SerializeField] private float m_aspect = 0.5f;
//
//         //Line draw example settings
//         [SerializeField] private Color m_bgColor = Color.black;
//         [SerializeField] private Color m_lineColor = Color.white;
//         [Range(0, 0.2f)] [SerializeField] private float m_lineWidth = 0.01f;
//
//         [SerializeField] private int m_pointsCount = 100;
//         [SerializeField] private float m_amplitude = 0.5f;
//         [SerializeField] private float m_frequency = 1f;
//         [SerializeField] private LineEndingType m_lineEndingType = LineEndingType.Butt;
//         [SerializeField] private LineJoinType m_lineJoinType = LineJoinType.Miter;
//         [SerializeField] private float m_speed = 1f;
//
//         private CanvasTexture m_canvasTexture;
//         private float m_time;
//
//         private async void Start()
//         {
//             m_canvasTexture = new CanvasTexture();
//             await MaterialProvider.Initialization;
//         }
//
//         private void Update()
//         {
//             if (!MaterialProvider.Initialization.IsCompleted)
//                 return;
//
//             //Prepare points list for line
//             m_time += Time.deltaTime * m_speed;
//             var sinePoints = GetSine(m_pointsCount, m_time, m_amplitude, m_frequency).TransformPoints(MathUtils.CreateMatrix2d_T(new float2(0, 0.5f)));
//
//             //Reinit texture with size
//             m_canvasTexture.Init(m_width, m_height);
//             //Set Aspect ratio
//             m_canvasTexture.AspectSettings.Aspect = m_aspect;
//             //Set background color
//             m_canvasTexture.ClearWithColor(m_bgColor);
//             //Draw the line
//             m_canvasTexture.DrawPolyLine(sinePoints, m_lineWidth, m_lineColor, m_lineEndingType, m_lineJoinType, 1f);
//             //Apply operations to the canvas texture
//             m_canvasTexture.Flush();
//
//             //Set texture to the renderer
//             m_testRenderer.material.mainTexture = m_canvasTexture.RenderTexture;
//         }
//
//         private List<float2> GetSine(float pointCount, float tOffset, float amplitude, float frequency)
//         {
//             var points = new List<float2>();
//             var step = 1f / pointCount;
//             for (int i = 0; i < pointCount; i++)
//             {
//                 var t = i * step * math.PI * 2f + tOffset;
//                 var x = 0.1f + (i * step) * 0.8f;
//                 var y = math.sin(t * frequency) * amplitude;
//                 points.Add(new float2(x, y));
//             }
//
//             return points;
//         }
//     }
// }