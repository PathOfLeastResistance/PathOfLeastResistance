using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public enum SignalQualityType
{
    Bad = 0,
    Medium = 1,
    Good = 2,
}

public class SignalsValidatorComponent : MonoBehaviour
{
    [SerializeField] private OscilloscopeComponent m_oscilloscopeReference;
    [SerializeField] private OscilloscopeComponent m_oscilloscopeSignal;
   
    [SerializeField] private float m_goodSignalThresh = 0.15f;
    [SerializeField] private float m_mediumSignalThresh = 0.3f;

    private bool m_refDone = false;
    private bool m_sigDone = false;

    private SignalQualityType m_signalQuality = SignalQualityType.Bad;

    public event Action<SignalQualityType> OnSignalValidated;
    
    public float? VoltageError { get; private set; }

    public float GoodSignalThresh => m_goodSignalThresh;
    
    public float MediumSignalThresh => m_mediumSignalThresh;
    
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

    public SignalQualityType SignalQuality
    {
        get => m_signalQuality;
        private set
        {
            m_signalQuality = value;
            OnSignalValidated?.Invoke(m_signalQuality);
        }
    }

    private void OnRecordFinish()
    {
        if (m_refDone && m_sigDone)
        {
            // get the maximum error between the two signals
            var errors = m_oscilloscopeReference.ActiveDataBuffer.Zip(m_oscilloscopeSignal.ActiveDataBuffer, (refData, sigData) => math.abs(refData.Voltage - sigData.Voltage));
            var maxError = errors.Max();
            VoltageError = maxError;
            if (maxError < m_goodSignalThresh)
            {
                SignalQuality = SignalQualityType.Good;
                Debug.Log($"Your signal is so... perfect!, the maximum error is {maxError}V");
            }
            else if (maxError < m_mediumSignalThresh)
            {
                SignalQuality = SignalQualityType.Medium;
                Debug.Log($"Your signal is not perfect, the maximum error is {maxError}V");
            }
            else
            {
                SignalQuality = SignalQualityType.Bad;
                Debug.Log($"Your signal is bad, the maximum error is {maxError}V");
            }
            

            m_refDone = false;
            m_sigDone = false;
        }
    }
}