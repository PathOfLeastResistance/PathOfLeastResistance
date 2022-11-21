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
    [SerializeField] private bool m_useTriggerLogic;

    [SerializeField] private List<OscilloscopeComponent> m_secondaryOscilloscopes;

    private ProbeElm m_probeElm;
    private bool m_isSecondary;

    protected override void PreInit()
    {
        foreach (var oscilloscope in m_secondaryOscilloscopes)
            oscilloscope.m_isSecondary = true;
    }

    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        m_probeElm = new ProbeElm(post0, post1);
        m_pin0.Init(() => post0);
        m_pin1.Init(() => post1);
        m_connectionsManager.Sim.AddElement(m_probeElm);

        m_connectionsManager.Sim.OnTimeStepHook += OnTick;

        m_canvasTexture = new CanvasTexture();
        m_canvasTexture.Init(m_pointsCountW, m_pointsCountH);
        m_renderTexture = m_canvasTexture.RenderTexture;
        m_screenRenderer.material.mainTexture = m_renderTexture;
    }

    [SerializeField] private RenderTexture m_renderTexture;
    [SerializeField] private Renderer m_screenRenderer;

    private int m_pointsCountW = 256;
    private int m_pointsCountH = 256;
    private DisplayData[] m_displayData = new DisplayData[256];

    private bool m_isRecording = false;
    private float m_recordPeriod = 1f;
    private float m_renderPeriod = 1f;
    private float m_triggeredTime = float.NegativeInfinity;

    private List<VoltageData> m_dataBuffer = new();
    private List<VoltageData> m_dataBackBuffer = new();

    private CanvasTexture m_canvasTexture;
    private float m_voltageHeight = 25f;
    private float m_voltageCenter = 0.5f;

    private float m_lastTick = float.NegativeInfinity;
    private float m_minStep = 1e-4f;

    private void OnTick()
    {
        // Take current time and skip too small ticks cause simulation tick is dynamic
        var time = (float)m_connectionsManager.Sim.t;
        if (time - m_lastTick < m_minStep)
            return;
        m_lastTick = time;

        // Record data if triggered
        if (m_isRecording)
        {
            if (time - m_triggeredTime > m_recordPeriod)
                StopRecord();
            else
                RecordPoint(time);
        }
        else
        {
            if (!m_isSecondary)
            {
                if (!m_useTriggerLogic || CheckTrigger())
                    StartRecord();
            }
        }
    }

    private float m_lastVoltage;
    private float m_triggerVoltage = 0.5f;

    private bool CheckTrigger()
    {
        var voltage = ReadVoltage();
        var wasTriggered = m_lastVoltage < m_triggerVoltage && voltage >= m_triggerVoltage;
        m_lastVoltage = voltage;
        return wasTriggered;
    }

    private void StartRecord()
    {
        foreach (var secondaryOscilloscope in m_secondaryOscilloscopes)
            secondaryOscilloscope.StartRecord();

        (m_dataBackBuffer, m_dataBuffer) = (m_dataBuffer, m_dataBackBuffer);
        m_dataBuffer.Clear();
        m_isRecording = true;
        m_triggeredTime = (float)m_connectionsManager.Sim.t;
    }

    private void RecordPoint(float time)
    {
        var data = new VoltageData()
        {
            Voltage = ReadVoltage(),
            Time = time,
        };
        m_dataBuffer.Add(data);
    }

    /// <summary>
    /// Reads current voltage on oscilloscope probes
    /// </summary>
    /// <returns></returns>
    private float ReadVoltage()
    {
        var isConnected = m_connectionsManager.HasWires(m_pin0) && m_connectionsManager.HasWires(m_pin1);
        return isConnected ? (float)m_probeElm.getVoltageDiff() : 0f;
    }

    /// <summary>
    /// Stops data recording
    /// </summary>
    private void StopRecord()
    {
        m_isRecording = false;
        m_triggeredTime = float.NegativeInfinity;
    }

    /// <summary>
    /// Prepares screenData
    /// </summary>
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
                var pixel = (int)(x * m_pointsCountW);

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
                var pixel = (int)(x * m_pointsCountW);
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
        for (int i = startFrom; i < m_pointsCountW; i++)
            m_displayData[i] = new DisplayData(0);
    }

    private void Update()
    {
        UpdateRenderData();
        DrawData();
    }

    private async void DrawData()
    {
        await MaterialProvider.Initialization;

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

        m_canvasTexture.Flush();
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_probeElm);
    }
}