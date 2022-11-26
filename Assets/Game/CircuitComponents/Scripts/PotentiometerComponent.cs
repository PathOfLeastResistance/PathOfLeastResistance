using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using UnityEngine;

public class PotentiometerComponent : CircuitComponent
{
    [SerializeField] private ConnectorPinBehaviour m_pin0;
    [SerializeField] private ConnectorPinBehaviour m_pin1;
    [SerializeField] private ConnectorPinBehaviour m_pin2;
    [SerializeField] private LinearSliderController m_sliderController;

    [SerializeField] private float m_resistance = 1000f;
    [Range(0f,1f)] [SerializeField] private float m_position = 0.5f;
    
    private PotElm m_potentiometer; 

    protected override void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();
        var post2 = m_uniquePostProvider.GetId();

        m_potentiometer = new PotElm(post0, post1, post2, m_resistance, m_position);
        m_pin0.Init(() => post0);
        m_pin1.Init(() => post1);
        m_pin2.Init(() => post2);
        m_connectionsManager.Sim.AddElement(m_potentiometer);

        m_sliderController.Position = m_position;
        m_sliderController.SubscribeHandle(OnSliderValueChanged);
    }
    
    private void OnSliderValueChanged(float value)
    {
        m_potentiometer.sliderPos = value;
    }

    protected override void DeinitComponent()
    {
        m_connectionsManager.Sim.RemoveElement(m_potentiometer);
    }
}
