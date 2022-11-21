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

    private bool m_RefDone = false;
    private bool m_SigDone = false;

    public event Action OnSignalValidated;

    private void Awake()
    {
        m_oscilloscopeReference.RecordFinishEvent += () =>
        {
            m_RefDone = true;
            OnRecordFinish();
        };
        m_oscilloscopeSignal.RecordFinishEvent += () =>
        {
            m_SigDone = true;
            OnRecordFinish();
        };
    }

    private void OnRecordFinish()
    {
        if (m_RefDone && m_SigDone)
        {
            // get the maximum error between the two signals
            var errors = m_oscilloscopeReference.ActiveDataBuffer.Zip(m_oscilloscopeSignal.ActiveDataBuffer, (refData, sigData) => math.abs(refData.Voltage - sigData.Voltage));
            var maxError = errors.Max();
            if (maxError < m_voltageTolerance)
            {
                OnSignalValidated?.Invoke();
                Debug.Log("Your signal is so... perfect!");
            }
            else
            {
                Debug.Log($"Your signal is not perfect, the maximum error is {maxError}V");
            }

            m_RefDone = false;
            m_SigDone = false;
        }
    }
}