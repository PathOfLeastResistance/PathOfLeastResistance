using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using Unity.Mathematics;
using UnityEngine;

public class VoltageComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_pin0;
    [SerializeField] private ConnectorPinBehaviour m_pin1;

    [SerializeField] private RotationHandle m_rotationHandle;
 
    [Range(0, 10)] [SerializeField] private float m_volts = 5f;

    private VoltageElm m_voltage;

    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        m_voltage = new VoltageElm(post0, post1);
        m_pin0.Init(() => post0);
        m_pin1.Init(() => post1);
        m_connectionsManager.Sim.AddElement(m_voltage);

        m_rotationHandle.Value = math.clamp(math.unlerp(0, 10, m_volts), 0, 1) ;
        m_rotationHandle.SubscribeValue(OnVoltageHandle);
    }
    
    private void OnVoltageHandle(float handlePosition)
    {
        m_voltage.MaxVoltage = math.lerp(0, 10, handlePosition);
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_voltage);
    }
}