using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoltageVisualizer : MonoBehaviour
{
    [SerializeField] private VoltmeterComponent m_voltmeter;
    [SerializeField] private TMPro.TMP_Text m_text;

    void Start() => m_text.text = ToVoltageText(0);

    void Update() => m_text.text = ToVoltageText(m_voltmeter.ReadVoltage());

    private string ToVoltageText(float voltage) => voltage.ToString("F2") + " V";
}