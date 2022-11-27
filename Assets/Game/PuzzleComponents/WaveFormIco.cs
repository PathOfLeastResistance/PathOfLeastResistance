using System;
using System.Collections;
using System.Collections.Generic;
using CircuitJSharp;
using UnityEngine;

public class WaveFormIco : MonoBehaviour
{
    private VoltageComponent m_voltageComponent;

    [SerializeField] private GameObject m_icoDC;
    [SerializeField] private GameObject m_icoAC;

    private void Awake()
    {
        m_voltageComponent = GetComponentInParent<VoltageComponent>();
        if (m_voltageComponent != null)
        {
            m_voltageComponent.SubscribeWaveFormChanged(OnWaveFormChanged);
        }
        else
        {
            Debug.LogWarning("Voltage component was not found", gameObject);
        }
    }

    private void OnWaveFormChanged(WaveForm wf)
    {
        m_icoDC.SetActive(wf == WaveForm.WF_DC);
        m_icoAC.SetActive(wf == WaveForm.WF_AC);
    }
}
