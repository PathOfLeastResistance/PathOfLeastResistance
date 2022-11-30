using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistorText : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text m_text;

    private void Awake()
    {
        GetComponentInParent<ResistorComponent>().SubscribeResistanceValue(OnResistanceChange);
    }
    
    private void OnResistanceChange(float resistance)
    {
        m_text.text = $"{resistance} Ω";
    }
}
