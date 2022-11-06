using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViJApps.CanvasTexture.Utils;

public class TextTest : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro m_text;
    [SerializeField] private float m_worldSize;
    private void Update()
    {
         m_text.fontSize = m_text.font.GetFontSizeToFitWorldSize(m_worldSize);
    }
}
