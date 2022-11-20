using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using UnityEngine;

public class PotentiometerComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_Pin0;
    [SerializeField] private ConnectorPinBehaviour m_Pin1;
    [SerializeField] private ConnectorPinBehaviour m_Pin2;

    [SerializeField] private float m_resistance;
    [Range(0f,1f)] [SerializeField] private float m_position;
    
    private PotElm m_potentiometer;

    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();
        var post2 = m_uniquePostProvider.GetId();

        m_potentiometer = new PotElm(post0, post1, post2, m_resistance, m_position);
        m_Pin0.Init(() => post0);
        m_Pin1.Init(() => post1);
        m_Pin2.Init(() => post2);
        m_connectionsManager.Sim.AddElement(m_potentiometer);
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_potentiometer);
    }
}
