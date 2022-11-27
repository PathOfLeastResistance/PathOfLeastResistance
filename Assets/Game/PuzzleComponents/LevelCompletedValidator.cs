using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class takes all SignalsValidatorComponent that are his children, and suscribes their OnSignalValidated event;
/// It throws a OnLevelCompleted event when all the signals are validated and have GOOD quality
/// </summary>
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

        Reset();
    }

    public void Reset()
    {
        m_checkSignals = true;
        m_allSignalsGood = false;
    }

    private void OnSignalReceived()
    {
        m_allSignalsGood = m_levelValidators.All(c=> c.SignalQuality == SignalQualityType.Good);
    }

    private void Update()
    {
        if (m_checkSignals)
        {
            if (m_allSignalsGood)
            {
                m_checkSignals = false;
                OnLevelCompleted?.Invoke();
                Debug.LogWarning("Level Completed");
            }
        }
    }
}
