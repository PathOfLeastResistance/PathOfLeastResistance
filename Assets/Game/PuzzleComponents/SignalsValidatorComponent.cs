using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class SignalsValidatorComponent : MonoBehaviour
{
    [SerializeField] private OscilloscopeComponent m_oscilloscopeReference;
    [SerializeField] private OscilloscopeComponent m_oscilloscopeSignal;
    [SerializeField] private float m_voltageTolerance = 0.1f;

    private bool m_refDone = false;
    private bool m_sigDone = false;

    private bool m_isSignalGood = false;

    public event Action<bool> OnSignalValidated;

    private void Awake()
    {
        m_oscilloscopeReference.RecordFinishEvent += () =>
        {
            m_refDone = true;
            OnRecordFinish();
        };
        m_oscilloscopeSignal.RecordFinishEvent += () =>
        {
            m_sigDone = true;
            OnRecordFinish();
        };
    }

    public bool IsSignalGood
    {
        get => m_isSignalGood;
        set
        {
            m_isSignalGood = value;
            OnSignalValidated?.Invoke(m_isSignalGood);
        }
    }

    private void OnRecordFinish()
    {
        if (m_refDone && m_sigDone)
        {
            // get the maximum error between the two signals
            var errors = m_oscilloscopeReference.ActiveDataBuffer.Zip(m_oscilloscopeSignal.ActiveDataBuffer, (refData, sigData) => math.abs(refData.Voltage - sigData.Voltage));
            var maxError = errors.Max();
            if (maxError < m_voltageTolerance)
            {
                IsSignalGood = true;
                Debug.Log("Your signal is so... perfect!");
            }
            else
            {
                IsSignalGood = false;
                Debug.Log($"Your signal is not perfect, the maximum error is {maxError}V");
            }

            m_refDone = false;
            m_sigDone = false;
        }
    }
}