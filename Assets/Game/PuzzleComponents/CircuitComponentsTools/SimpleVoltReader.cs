using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoltReader : MonoBehaviour
{
    [SerializeField] private VoltmeterComponent m_voltmeter;

    [SerializeField] private TMPro.TMP_Text m_text;
    private void Update()
    {
        m_text.text = $"{m_voltmeter.ReadVoltage():N2}V" ;
    }
}
