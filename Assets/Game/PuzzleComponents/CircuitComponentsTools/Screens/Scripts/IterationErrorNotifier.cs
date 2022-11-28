using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterationErrorNotifier : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text m_lastErrorText;
    [SerializeField] private TMPro.TMP_Text m_maxErrorText;


    [SerializeField] private SignalsValidatorComponent m_signalValidator;

    [SerializeField] private Color m_goodColor;
    [SerializeField] private Color m_mediumColor;
    [SerializeField] private Color m_badColor;

    private void Awake()
    {
        m_signalValidator.OnSignalValidated += OnSignalValidated;
        m_maxErrorText.text = $"Max allowed error: {m_signalValidator.GoodSignalThresh: 0.00} V";
    }

    private void OnDestroy()
    {
        m_signalValidator.OnSignalValidated -= OnSignalValidated;
    }

    private void OnSignalValidated(SignalQualityType quality)
    {
        var error = m_signalValidator.VoltageError;
        m_lastErrorText.color = GetQualityColor(quality);
        m_lastErrorText.text = error.HasValue ? $"Last signal error: {error:0.00} V" : "Last signal error: N/A";
    }

    private Color GetQualityColor(SignalQualityType quality)
    {
        switch (quality)
        {
            case SignalQualityType.Good:
                return m_goodColor;
            case SignalQualityType.Medium:
                return m_mediumColor;
            case SignalQualityType.Bad:
                return m_badColor;
            default:
                throw new ArgumentOutOfRangeException(nameof(quality), quality, null);
        }
    }
}