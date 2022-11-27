using System;
using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;

public class VoltageComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_pin0;
    [SerializeField] private ConnectorPinBehaviour m_pin1;

    [Range(0, 10)] [SerializeField] private float m_volts = 5f;
    [Range(1, 50)] [SerializeField] private float m_frequency = 10f;
    [SerializeField] private WaveForm m_waveForm = WaveForm.WF_DC;

    private VoltageElm m_voltageElm;

    private event Action<float> OnVoltageChanged;
    private event Action<float> OnFrequencyChanged;
    private event Action<WaveForm> OnWaveFormChanged;

    public float MaxVoltage
    {
        get => m_volts;
        set
        {
            m_volts = value;
            m_voltageElm.MaxVoltage = m_volts;
            OnVoltageChanged?.Invoke(m_volts);
        }
    }

    public float Frequency
    {
        get => m_frequency;
        set
        {
            m_frequency = value;
            m_voltageElm.Frequency = m_frequency;
            OnFrequencyChanged?.Invoke(m_frequency);
        }
    }

    public WaveForm WaveForm
    {
        get => m_waveForm;
        set
        {
            m_waveForm = value;
            m_voltageElm.Waveform = m_waveForm;
            OnWaveFormChanged?.Invoke(m_waveForm);
        }
    }

    public IDisposable SubscribeWaveFormChanged(Action<WaveForm> waveFormHandler)
    {
        OnWaveFormChanged += waveFormHandler;
        waveFormHandler?.Invoke(m_waveForm);
        return new DisposableAction(() => OnWaveFormChanged -= waveFormHandler);
    }
    
    public IDisposable SubscribeFrequencyChanged(Action<float> frequencyHandler)
    {
        OnFrequencyChanged += frequencyHandler;
        frequencyHandler?.Invoke(m_frequency);
        return new DisposableAction(() => OnFrequencyChanged -= frequencyHandler);
    }
    
    public IDisposable SubscribeVoltageChanged(Action<float> voltageHandler)
    {
        OnVoltageChanged += voltageHandler;
        voltageHandler?.Invoke(m_volts);
        return new DisposableAction(() => OnVoltageChanged -= voltageHandler);
    }

    protected override void InitComponent()
    {
        // Init pins
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();
        m_pin0.Init(() => post0);
        m_pin1.Init(() => post1);

        // Init component
        m_voltageElm = new VoltageElm(post0, post1, m_waveForm);
        m_connectionsManager.Sim.AddElement(m_voltageElm);
        m_voltageElm.MaxVoltage = m_volts;
        m_voltageElm.Frequency = m_frequency;
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_voltageElm);
    }
}