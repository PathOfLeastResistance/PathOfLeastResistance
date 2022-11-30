using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class takes all SignalsValidatorComponent that are his children, and suscribes their OnSignalValidated event;
/// It throws a OnLevelCompleted event when all the signals are validated and have GOOD quality
/// </summary>
public class LevelCompletedValidator : MonoBehaviour
{
    public event Action OnLevelCompleted;

    private SignalsValidatorComponent[] m_LevelValidators;
    private bool m_AllSignalsGood = false;
    private bool m_CheckSignals = true;

    private void Start()
    {
        m_LevelValidators = GetComponentsInChildren<SignalsValidatorComponent>();
        foreach (var validator in m_LevelValidators)
            validator.OnSignalValidated += (c) => OnSignalReceived();

        Reset();
    }

    public void Reset()
    {
        m_CheckSignals = true;
        m_AllSignalsGood = false;
    }

    private void OnSignalReceived()
    {
        m_AllSignalsGood = m_LevelValidators.All(c => c.SignalQuality == SignalQualityType.Good);
    }

    private void Update()
    {
        
#if UNITY_EDITOR
        //Winner 
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            m_AllSignalsGood = true;
#endif
        
        if (m_CheckSignals)
        {
            if (m_AllSignalsGood)
            {
                m_CheckSignals = false;
                OnLevelCompleted?.Invoke();
                Debug.LogWarning("Level Completed");
            }
        }
    }
}