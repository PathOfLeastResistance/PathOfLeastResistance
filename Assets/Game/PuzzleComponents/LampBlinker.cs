using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LampBlinker : MonoBehaviour
{
    private LevelCompletedValidator m_validator;
    
    [SerializeField] private Material m_enabledMaterial;
    [SerializeField] private Material m_disabledMaterial;

    [SerializeField] private int m_initialCount;

    private List<Renderer> m_lamps = new List<Renderer>();

    private void Awake()
    {
        m_validator = GetComponentInParent<LevelCompletedValidator>();
        
        foreach (var lamp in m_lamps)
            lamp.sharedMaterial = m_disabledMaterial;
        for (int i = 0; i < m_initialCount; i++)
            m_lamps[i].sharedMaterial = m_enabledMaterial;
        
        m_validator.OnLevelCompleted+=() => m_lamps[m_initialCount].sharedMaterial = m_enabledMaterial;
    }
}
