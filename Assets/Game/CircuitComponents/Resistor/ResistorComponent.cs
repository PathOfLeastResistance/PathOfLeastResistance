using CircuitJSharp;
using UnityEngine;

public class ResistorComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_Pin0;
    [SerializeField] private ConnectorPinBehaviour m_Pin1;

    [SerializeField] private float m_resistance;
    
    private ResistorElm m_resistor;

    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        m_resistor = new ResistorElm(post0, post1, m_resistance);
        m_Pin0.Init(() => post0);
        m_Pin1.Init(() => post1);
        m_connectionsManager.Sim.AddElement(m_resistor);
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_resistor);
    }
}