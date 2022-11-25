using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ViJApps.CanvasTexture;

public class OscilloscopeScreen : MonoBehaviour
{
    private const int ScreenResolution = 128;

    [SerializeField] private RenderTexture m_renderTexture;
    [SerializeField] private Renderer m_screenRenderer;
    [SerializeField] private OscilloscopeComponent m_oscilloscope;

    private CanvasTexture m_canvasTexture;
    private int m_pointsCountW = ScreenResolution;
    private int m_pointsCountH = ScreenResolution;

    private DisplayData[] m_displayData = new DisplayData[ScreenResolution];

    // TODO: make it visual settings
    private float m_voltageHeight = 10f;
    private float m_voltageCenter = 0.5f;
    private float m_renderPeriod = 1f;

    private int m_BuffersEdgePixel;
    
    public event Action<RenderTexture> OnRenderTextureChanged;
    
    public RenderTexture RenderTexture
    {
        get => m_renderTexture;
        private set
        {
            m_renderTexture = value;
            OnRenderTextureChanged?.Invoke(m_renderTexture);
        }
    }
    
    private async void Awake()
    {
        await MaterialProvider.Initialization;
        m_canvasTexture = new CanvasTexture();
        var desc = new RenderTextureDescriptor(m_pointsCountW, m_pointsCountH);
        m_canvasTexture.Init(desc);
        RenderTexture = m_canvasTexture.RenderTexture;
        RenderTexture.filterMode = FilterMode.Point;

        if (m_screenRenderer != null)
            m_screenRenderer.material.mainTexture = RenderTexture;
    }

    private async void Update()
    {
        await MaterialProvider.Initialization;
        UpdateRenderData();
        DrawData();
    }

    /// <summary>
    /// Prepares screenData
    /// What we do here:
    /// First, we take the primary buffer pixels.
    /// If there is not enough data, we fill the rest with back buffer pixels.
    /// If there is not enough data again, we fill the rest with zero voltage.
    ///
    /// the pixel height is bounding box of all voltages in the time interval of this pixel
    ///
    /// TODO: Remove copy-paste duplications of primary and back buffers
    /// </summary>
    private void UpdateRenderData()
    {
        DisplayData pixelData;

        var maxPixel = -1;
        var activeDataBuffer = m_oscilloscope.ActiveDataBuffer;
        var dataBackBuffer = m_oscilloscope.BackDataBuffer;

        //PrimaryBuffer
        if (activeDataBuffer.Count != 0)
        {
            var startPoint = activeDataBuffer[0].Time;
            for (int i = 0; i < activeDataBuffer.Count; i++)
            {
                var data = activeDataBuffer[i];
                var x = (data.Time - startPoint) / m_renderPeriod;
                var y = data.Voltage;
                var pixel = (int)(x * m_pointsCountW);

                if (pixel == m_pointsCountW)
                    break;

                if (pixel > maxPixel)
                {
                    maxPixel = pixel;
                    pixelData = new DisplayData(y);
                }
                else
                {
                    pixelData = m_displayData[pixel];
                    pixelData.EncapsulateVoltage(y);
                }

                m_displayData[pixel] = pixelData;
            }
        }

        var lastPixel = maxPixel;
        m_BuffersEdgePixel = lastPixel;

        //BackBuffer
        //TODO: binary search to find first pixel to start from
        if (dataBackBuffer.Count != 0)
        {
            var startPoint = dataBackBuffer[0].Time;
            for (int i = 0; i < dataBackBuffer.Count; i++)
            {
                var data = dataBackBuffer[i];
                var x = (data.Time - startPoint) / m_renderPeriod;
                var y = data.Voltage;
                var pixel = (int)(x * m_pointsCountW);
                if (pixel < lastPixel)
                    continue;
                if (pixel == m_pointsCountW)
                    break;

                if (pixel > maxPixel)
                {
                    maxPixel = pixel;
                    pixelData = new DisplayData(y);
                }
                else
                {
                    pixelData = m_displayData[pixel];
                    pixelData.EncapsulateVoltage(y);
                }

                m_displayData[pixel] = pixelData;
            }
        }

        //NoData
        var startFrom = math.max(0, maxPixel);
        for (int i = startFrom; i < m_pointsCountW; i++)
            m_displayData[i] = new DisplayData(0);
    }

    private void DrawData()
    {
        m_canvasTexture.ClearWithColor(Color.black);
        for (int i = 0; i < m_pointsCountW; i++)
        {
            var data = m_displayData[i];
            //the position is half pixel cause of current paint algorithm;
            var xPosition = new float2(i + 0.5f, data.Center / m_voltageHeight * m_pointsCountH + m_voltageCenter * m_pointsCountH);

            //the min size is clamped to 1 pixel of the oscilloscope
            var size = new float2(1, math.max(data.Height / m_voltageHeight * m_pointsCountH, 1));
            m_canvasTexture.DrawRectPixels(xPosition, size, Color.white);
        }

        m_canvasTexture.DrawRectPixels(new float2(m_BuffersEdgePixel + 0.5f, m_pointsCountH / 2f), new float2(1, m_pointsCountH * 2), Color.red);
        m_canvasTexture.Flush();
    }
}