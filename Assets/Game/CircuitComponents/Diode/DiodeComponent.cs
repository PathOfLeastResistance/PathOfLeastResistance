using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using UnityEngine;

public class DiodeComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_Pin0;
    [SerializeField] private ConnectorPinBehaviour m_Pin1;

    private DiodeElm m_diode;

    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        m_diode = new DiodeElm(post0, post1);
        m_Pin0.Init(() => post0);
        m_Pin1.Init(() => post1);
        m_connectionsManager.Sim.AddElement(m_diode);
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_diode);
    }
}