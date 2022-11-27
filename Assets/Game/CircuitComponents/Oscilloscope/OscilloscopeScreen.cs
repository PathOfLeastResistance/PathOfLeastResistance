using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ViJApps.CanvasTexture;


[Serializable]
public struct OscilloscopeDataSettings
{
    public float MinVoltage;
    public float MaxVoltage;
    public float RenderPeriod;
}

public class OscilloscopeScreen : MonoBehaviour
{
    private const int ScreenResolutionX = 320 / 2;
    private const int ScreenResolutionY = 240 / 2;

    [Header("Optional renderer for debug")] [SerializeField]
    private Renderer m_screenRenderer;

    [SerializeField] private RenderTexture m_renderTexture;
    [SerializeField] private OscilloscopeComponent m_oscilloscope;
    [SerializeField] private bool m_drawScanner = false;

    private CanvasTexture m_canvasTexture;
    private int m_pointsCountW = ScreenResolutionX;
    private int m_pointsCountH = ScreenResolutionY;
    private int m_buffersEdgePixel;

    private DisplayData[] m_displayData = new DisplayData[ScreenResolutionX];
    private List<(int, int2)> m_pixelsToDraw = new List<(int, int2)>(ScreenResolutionX);
    private List<(int, int2)> m_scanner = new List<(int, int2)>(1);

    [SerializeField] private OscilloscopeDataSettings m_settings = new OscilloscopeDataSettings
    {
        MinVoltage = -5,
        MaxVoltage = 5,
        RenderPeriod = 0.5f
    };

    public OscilloscopeDataSettings Settings
    {
        get => m_settings;
        set => m_settings = value;
    }

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
                var x = (data.Time - startPoint) / m_settings.RenderPeriod;
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

        //Encapsulate previous data to remove gaps between current pixel and previous pixel
        for (int i = 1; i < maxPixel; i++)
        {
            //check if two ranges are not intersected and make them intersect 
            if (m_displayData[i].MaxVoltage < m_displayData[i - 1].MinVoltage)
                m_displayData[i].EncapsulateVoltage(m_displayData[i - 1].MinVoltage);
            else if (m_displayData[i].MinVoltage > m_displayData[i - 1].MaxVoltage)
                m_displayData[i].EncapsulateVoltage(m_displayData[i - 1].MaxVoltage);
        }

        var lastPixel = maxPixel;
        m_buffersEdgePixel = lastPixel;

        //BackBuffer
        //TODO: binary search to find first pixel to start from
        if (dataBackBuffer.Count != 0)
        {
            var startPoint = dataBackBuffer[0].Time;
            for (int i = 0; i < dataBackBuffer.Count; i++)
            {
                var data = dataBackBuffer[i];
                var x = (data.Time - startPoint) / m_settings.RenderPeriod;
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

        //Encapsulate previous data to remove gaps between current pixel and previous pixel
        if (lastPixel < 1)
            lastPixel = 1;
        for (int i = lastPixel + 1; i < maxPixel; i++)
        {
            //check if two ranges are not intersected and make them intersect 
            if (m_displayData[i].MaxVoltage < m_displayData[i - 1].MinVoltage)
                m_displayData[i].EncapsulateVoltage(m_displayData[i - 1].MinVoltage);
            else if (m_displayData[i].MinVoltage > m_displayData[i - 1].MaxVoltage)
                m_displayData[i].EncapsulateVoltage(m_displayData[i - 1].MaxVoltage);
        }

        //NoData. Fill with zero voltage
        var startFrom = math.max(0, maxPixel);
        for (int i = startFrom; i < m_pointsCountW; i++)
            m_displayData[i] = new DisplayData(0);
    }

    /// <summary>
    /// We use red channel to encode voltage
    /// We use green channel to encode scanner position
    /// </summary>
    private void DrawData()
    {
        m_canvasTexture.ClearWithColor(Color.black);

        //Prepare pixels of signal
        m_pixelsToDraw.Clear();
        for (int i = 0; i < m_pointsCountW; i++)
        {
            var data = m_displayData[i];
            var minPixel = (int)math.floor(math.remap(m_settings.MinVoltage, m_settings.MaxVoltage, 0, m_pointsCountH - 1, data.MinVoltage));
            var maxPixel = (int)math.floor(math.remap(m_settings.MinVoltage, m_settings.MaxVoltage, 0, m_pointsCountH - 1, data.MaxVoltage));

            m_pixelsToDraw.Add(new(i, new int2(minPixel, maxPixel)));
        }

        m_canvasTexture.DrawColumns(m_pixelsToDraw, Color.red);

        //Prepare pixels of scanner
        if (m_drawScanner)
        {
            m_scanner.Clear();
            m_scanner.Add((m_buffersEdgePixel, new int2(0, m_pointsCountH)));
            m_canvasTexture.DrawColumns(m_scanner, Color.green);
        }

        //Apply
        m_canvasTexture.Flush();
    }
}