using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using Unity.Mathematics;
using UnityEngine;

public class VoltageComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_pin0;
    [SerializeField] private ConnectorPinBehaviour m_pin1;

    [Range(0, 10)] [SerializeField] private float m_volts = 5f;
    [Range(1, 50)] [SerializeField] private float m_frequency = 10f;
    [SerializeField] private WaveForm m_waveForm = WaveForm.WF_DC;

    private VoltageElm m_voltageElm;

    public float MaxVoltage
    {
        get => m_volts;
        set
        {
            m_volts = value;
            m_voltageElm.MaxVoltage = m_volts;
        }
    }

    public float Frequency
    {
        get => m_frequency;
        set
        {
            m_frequency = value;
            m_voltageElm.Frequency = m_frequency;
        }
    }
    
    public WaveForm WaveForm
    {
        get => m_waveForm;
        set
        {
            m_waveForm = value;
            m_voltageElm.Waveform = m_waveForm;
        }
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