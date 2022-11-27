using System;
using CircuitJSharp;
using UnityEngine;
using UnityTools;

public class ResistorComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_Pin0;
    [SerializeField] private ConnectorPinBehaviour m_Pin1;

    [SerializeField] private float m_resistance;
    
    private ResistorElm m_resistorElm;
    private event Action<float> m_resistanceChanged;
    
    public float Resistance
    {
        get => m_resistance;
        set
        {
            m_resistance = value;
            m_resistorElm.Resistance = m_resistance;
            m_resistanceChanged?.Invoke(m_resistance);
        }
    }
     
    public IDisposable SubscribeResistanceValue(Action<float> resistanceHandler)
    {
        m_resistanceChanged += resistanceHandler;
        resistanceHandler?.Invoke(m_resistance);
        return new DisposableAction(()=> m_resistanceChanged -= resistanceHandler);
    }

    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        m_resistorElm = new ResistorElm(post0, post1, m_resistance);
        m_Pin0.Init(() => post0);
        m_Pin1.Init(() => post1);
        m_connectionsManager.Sim.AddElement(m_resistorElm);
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_resistorElm);
    }
}