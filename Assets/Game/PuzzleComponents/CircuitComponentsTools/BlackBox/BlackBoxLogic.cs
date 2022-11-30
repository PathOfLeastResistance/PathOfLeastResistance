using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Zenject;

public class BlackBoxLogic : MonoBehaviour
{
    private LevelCompletedValidator m_validator;

    [SerializeField] private Material m_enabledMaterial;
    [SerializeField] private Material m_disabledMaterial;

    [SerializeField] private int m_initialCount;

    [SerializeField] private List<Renderer> m_lamps = new List<Renderer>();

    [SerializeField] PlayableDirector m_playableDirector;

    private void Awake()
    {
        m_validator = GetComponentInParent<LevelCompletedValidator>();

        foreach (var lamp in m_lamps)
            lamp.sharedMaterial = m_disabledMaterial;
        for (int i = 0; i < m_initialCount; i++)
            m_lamps[i].sharedMaterial = m_enabledMaterial;

        m_validator.OnLevelCompleted += OnLevelCompleted;
    }

    private void OnLevelCompleted()
    {
        if (m_initialCount == m_lamps.Count - 1)
            m_playableDirector.Play();
        m_lamps[m_initialCount].sharedMaterial = m_enabledMaterial;
    }
}