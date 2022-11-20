using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircuitJSharp;
using Unity.Mathematics;
using ViJApps.CanvasTexture;

public struct VoltageData
{
    public float Voltage;
    public float Time;
}

public struct DisplayData
{
    public float MinVoltage { get; private set; }
    public float MaxFoltage { get; private set; }

    public DisplayData(float initialValue)
    {
        MinVoltage = initialValue;
        MaxFoltage = initialValue;
    }

    public float Height => MaxFoltage - MinVoltage;

    public float Center => (MaxFoltage + MinVoltage) / 2;

    public void Update(float value)
    {
        if (value < MinVoltage)
            MinVoltage = value;
        if (value > MaxFoltage)
            MaxFoltage = value;
    }
}

public class OscilloscopeComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_pin0;
    [SerializeField] private ConnectorPinBehaviour m_pin1;

    [SerializeField] private ButtonController m_testBtn;
    [SerializeField] private RenderTexture m_renderTexture;
    [SerializeField] private Renderer m_screenRenderer;

    private ProbeElm m_probeElm;

    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        m_probeElm = new ProbeElm(post0, post1);
        m_pin0.Init(() => post0);
        m_pin1.Init(() => post1);
        m_connectionsManager.Sim.AddElement(m_probeElm);

        m_connectionsManager.Sim.OnTimeStepHook += OnTick;
        m_testBtn.PressEvent += StartRecord;
        StartRecord();

        m_canvasTexture = new CanvasTexture();
        m_canvasTexture.Init(256, 256);
        m_renderTexture = m_canvasTexture.RenderTexture;
        m_screenRenderer.material.mainTexture = m_renderTexture;
    }

    private bool m_isRecording = false;
    private float m_recordPeriod = 1f;
    private float m_renderPeriod = 1f;
    private float m_triggeredTime = float.NegativeInfinity;

    List<VoltageData> m_dataBuffer = new List<VoltageData>();
    private List<VoltageData> m_dataBackBuffer = new List<VoltageData>();

    private DisplayData[] m_displayData = new DisplayData[256];
    private int m_pointCount = 256;

    private void OnTick()
    {
        var time = (float)m_connectionsManager.Sim.t;
        if (m_isRecording)
        {
            if (time - m_triggeredTime > m_recordPeriod)
                StopRecord();
            else
                RecordPoint(time);
        }
    }

    private void RecordPoint(float time)
    {
        var data = new VoltageData()
        {
            Voltage = (float)m_probeElm.getVoltageDiff(),
            Time = time,
        };

        m_dataBuffer.Add(data);
    }

    private void StopRecord()
    {
        m_isRecording = false;
        m_triggeredTime = float.NegativeInfinity;

        UpdateRenderData();
        DrawData();

        //TODO: replace with trigger
        StartRecord();
    }

    private void StartRecord()
    {
        (m_dataBackBuffer, m_dataBuffer) = (m_dataBuffer, m_dataBackBuffer);
        m_dataBuffer.Clear();
        m_isRecording = true;
        m_triggeredTime = (float)m_connectionsManager.Sim.t;
    }

    private void UpdateRenderData()
    {
        var maxPixel = -1;
        DisplayData pixelData;

        //PrimaryBuffer
        if (m_dataBuffer.Count != 0)
        {
            var startPoint = m_dataBuffer[0].Time;
            for (int i = 0; i < m_dataBuffer.Count; i++)
            {
                var data = m_dataBuffer[i];
                var x = (data.Time - startPoint) / m_renderPeriod;
                var y = data.Voltage;
                var pixel = (int)(x * m_pointCount);

                if (pixel > maxPixel)
                {
                    maxPixel = pixel;
                    pixelData = new DisplayData(y);
                }
                else
                {
                    pixelData = m_displayData[pixel];
                    pixelData.Update(y);
                }

                m_displayData[pixel] = pixelData;
            }
        }

        var lastPixel = maxPixel;

        //BackBuffer
        //TODO: binary search to find first pixel to start from
        if (m_dataBackBuffer.Count != 0)
        {
            var startPoint = m_dataBackBuffer[0].Time;
            for (int i = 0; i < m_dataBackBuffer.Count; i++)
            {
                var data = m_dataBackBuffer[i];
                var x = (data.Time - startPoint) / m_renderPeriod;
                var y = data.Voltage;
                var pixel = (int)(x * m_pointCount);
                if (pixel < lastPixel)
                    continue;

                if (pixel > maxPixel)
                {
                    maxPixel = pixel;
                    pixelData = new DisplayData(y);
                }
                else
                {
                    pixelData = m_displayData[pixel];
                    pixelData.Update(y);
                }

                m_displayData[pixel] = pixelData;
            }
        }

        //NoData
        var startFrom = math.max(0, maxPixel);
        for (int i = startFrom; i < m_pointCount; i++)
        {
            m_displayData[i] = new DisplayData(0);
        }
    }

    private void Update()
    {
        UpdateRenderData();
        DrawData();
    }

    private CanvasTexture m_canvasTexture;
    private float m_voltageHeight = 25f;
    private float m_voltageCenter = 0.5f;

    private async void DrawData()
    {
        await MaterialProvider.Initialization;

        m_canvasTexture.ClearWithColor(Color.black);
        for (int i = 0; i < 256; i++)
        {
            var data = m_displayData[i];
            //the position is half pixel cause of current paint algorithm;
            var xPosition = new float2(i + 0.5f, data.Center / m_voltageHeight * 256 + m_voltageCenter * 256);

            //the min size is clamped to 1 pixel of the oscilloscope
            var size = new float2(1, math.max(data.Height / m_voltageHeight * 256, 1));
            m_canvasTexture.DrawRectPixels(xPosition, size, Color.white);
        }

        m_canvasTexture.Flush();
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_probeElm);
    }
}