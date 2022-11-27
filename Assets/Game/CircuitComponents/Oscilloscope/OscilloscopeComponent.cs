using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircuitJSharp;
using Unity.Mathematics;
using ViJApps.CanvasTexture;
using System;

public struct VoltageData
{
    public float Voltage;
    public float Time;
}

public class OscilloscopeComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_pin0;
    [SerializeField] private ConnectorPinBehaviour m_pin1;
    [SerializeField] private bool m_useTriggerLogic;

    [SerializeField] private List<OscilloscopeComponent> m_secondaryOscilloscopes;

    private ProbeElm m_probeElm;
    private bool m_isSecondary;

    //Trigger and data collection
    private List<VoltageData> m_activeDataBuffer = new(5000);
    private List<VoltageData> m_backDataBuffer = new(5000);
    private bool m_isRecording = false;
    private float m_recordPeriod = 0.5f;
    private float m_triggeredTime = float.NegativeInfinity;
    private float m_lastTick = float.NegativeInfinity;
    private float m_minStep = 1e-4f;
    private float m_lastVoltage;
    private float m_triggerVoltage = 0.5f;

    public ConnectorPinBehaviour Pin0 => m_pin0;

    public ConnectorPinBehaviour Pin1 => m_pin1;

    /// <summary>
    /// The primary buffer. Actual recorded data. we swap buffers in StartRecord method.
    /// </summary>
    public IReadOnlyList<VoltageData> ActiveDataBuffer => m_activeDataBuffer;

    /// <summary>
    /// The backbuffer. Contains previous written data. we swap buffers in StartRecord method.
    /// </summary>
    public IReadOnlyList<VoltageData> BackDataBuffer => m_backDataBuffer;

    public event Action RecordFinishEvent;

    protected override void InitComponent()
    {
        foreach (var oscilloscope in m_secondaryOscilloscopes)
            oscilloscope.m_isSecondary = true;
        
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        m_probeElm = new ProbeElm(post0, post1);
        m_pin0.Init(() => post0);
        m_pin1.Init(() => post1);
        m_connectionsManager.Sim.AddElement(m_probeElm);
        m_connectionsManager.Sim.OnTimeStepHook += OnTick;
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_probeElm);
        m_connectionsManager.Sim.OnTimeStepHook -= OnTick;
    }

    /// <summary>
    /// Oscilloscope tick, called every time step in CirSim hook
    /// Writes data, checks triggers, etc
    /// </summary>
    private void OnTick()
    {
        // Take current time and skip too small ticks cause simulation tick is dynamic
        var time = (float)m_connectionsManager.Sim.t;
        if (time - m_lastTick < m_minStep)
            return;
        m_lastTick = time;

        // Record data if triggered
        if (!m_isSecondary)
        {
            if (m_isRecording)
            {
                if (time - m_triggeredTime > m_recordPeriod)
                    StopRecord();
                else
                    RecordPoint(time);
            }
            else
            {
                if (!m_useTriggerLogic || CheckTrigger())
                    StartRecord();
            }
        }
    }

    /// <summary>
    /// Checks if trigger can be activated.
    /// Current implementations checks if voltage is above trigger voltage. and if it was below before.
    /// </summary>
    /// <returns></returns>
    private bool CheckTrigger()
    {
        var voltage = ReadVoltage();
        var wasTriggered = m_lastVoltage < m_triggerVoltage && voltage >= m_triggerVoltage;
        m_lastVoltage = voltage;
        return wasTriggered;
    }

    private void StartRecord()
    {
        (m_backDataBuffer, m_activeDataBuffer) = (m_activeDataBuffer, m_backDataBuffer);
        m_activeDataBuffer.Clear();
        m_isRecording = true;
        m_triggeredTime = (float)m_connectionsManager.Sim.t;
        
        foreach (var secondaryOscilloscope in m_secondaryOscilloscopes)
            secondaryOscilloscope.StartRecord();
    }

    private void RecordPoint(float time)
    {
#if UNITY_EDITOR
        if (!m_isRecording)
            throw new Exception("How did you get here?");
#endif

        var data = new VoltageData()
        {
            Voltage = ReadVoltage(),
            Time = time,
        };
        m_activeDataBuffer.Add(data);

        foreach (var secondaryOscilloscope in m_secondaryOscilloscopes)
            secondaryOscilloscope.RecordPoint(time);
    }

    /// <summary>
    /// Reads current voltage on oscilloscope probes. validates if both connectors are not floating
    /// </summary>
    /// <returns></returns>
    public float ReadVoltage()
    {
        var isConnected = m_connectionsManager.HasWires(m_pin0) && m_connectionsManager.HasWires(m_pin1);
        return isConnected ? (float)m_probeElm.getVoltageDiff() : 0f;
    }

    // private void Update()
    // {
    //     Debug.Log(ReadVoltage());
    // }

    /// <summary>
    /// Stops data recording and invokes <see cref="RecordFinishEvent"/>
    /// </summary>
    private void StopRecord()
    {
        if (m_isRecording)
        {
            m_isRecording = false;
            m_triggeredTime = float.NegativeInfinity;
            RecordFinishEvent?.Invoke();
        }
        
        foreach (var secondaryOscilloscope in m_secondaryOscilloscopes)
            secondaryOscilloscope.StopRecord();
    }
}