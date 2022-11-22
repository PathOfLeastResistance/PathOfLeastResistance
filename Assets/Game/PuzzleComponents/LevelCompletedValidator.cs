using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelCompletedValidator : MonoBehaviour
{
    public event Action OnLevelCompleted;

    private SignalsValidatorComponent[] m_levelValidators;
    private bool m_allSignalsGood = false;
    private bool m_checkSignals = true;
    
    private void Start()
    {
        m_levelValidators = GetComponentsInChildren<SignalsValidatorComponent>();
        foreach (var validator in m_levelValidators)
            validator.OnSignalValidated += (c) => OnSignalReceived();
    }

    private void OnSignalReceived()
    {
        m_allSignalsGood = m_levelValidators.All(c=> c.IsSignalGood);
    }

    private void Update()
    {
        if (m_checkSignals)
        {
            if (m_allSignalsGood)
            {
                m_checkSignals = false;
                OnLevelCompleted?.Invoke();
                Debug.LogWarning("Completed");
            }
        }
    }
}
