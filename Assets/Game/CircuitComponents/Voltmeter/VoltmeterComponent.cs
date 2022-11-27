using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using UnityEngine;

public class VoltmeterComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_pin0;
    [SerializeField] private ConnectorPinBehaviour m_pin1;
    
    private ProbeElm m_probeElm;
    
    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        m_probeElm = new ProbeElm(post0, post1);
        m_pin0.Init(() => post0);
        m_pin1.Init(() => post1);
        m_connectionsManager.Sim.AddElement(m_probeElm);
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_probeElm);
    }
    
    /// <summary>
    /// Reads current voltage on voltmeter probes. validates if both connectors are not floating
    /// </summary>
    /// <returns></returns>
    public float ReadVoltage()
    {
        var isConnected = m_connectionsManager.HasWires(m_pin0) && m_connectionsManager.HasWires(m_pin1);
        return isConnected ? (float)m_probeElm.getVoltageDiff() : 0f;
    }
}
