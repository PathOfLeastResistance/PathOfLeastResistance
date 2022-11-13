using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using CircuitJSharp;

public class CircuitComponent : MonoBehaviour
{
    [Inject] private ConnectionManager m_connectionsManager;
    [Inject] private UniquePostProvider m_uniquePostProvider;

    [SerializeField] private List<ConnectorPinBehaviour> m_pins = new List<ConnectorPinBehaviour>();

    protected virtual void InitComponent()
    {
        var post0 = m_uniquePostProvider.GetId();
        var post1 = m_uniquePostProvider.GetId();

        var resistor = new ResistorElm(post0, post1);
        m_pins[0].Init(() => post0);
        m_pins[1].Init(() => post1);
        m_connectionsManager.Sim.AddElement(resistor);
    }

    private void RegisterPins()
    {
        foreach (var pin in m_pins)
            m_connectionsManager.RegisterPin(pin);
    }

    private void Init()
    {
        InitComponent();
        RegisterPins();
    }

    private void Start()
    {
        Init();
    }
}