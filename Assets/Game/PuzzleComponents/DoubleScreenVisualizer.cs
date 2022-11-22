using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleScreenVisualizer : MonoBehaviour
{
    [SerializeField] private OscilloscopeScreen m_screen1;
    [SerializeField] private OscilloscopeScreen m_screen2;

    [SerializeField] private Renderer m_renderer;

    private void Awake()
    {
        SetFirstTexture(m_screen1.RenderTexture);
        SetSecondTexture(m_screen2.RenderTexture);

        m_screen1.OnRenderTextureChanged += SetFirstTexture;
        m_screen2.OnRenderTextureChanged += SetSecondTexture;
    }

    private void SetFirstTexture(RenderTexture texture)
    {
        m_renderer.material.SetTexture("_Screen1", texture);
    }

    private void SetSecondTexture(RenderTexture texture)
    {
        m_renderer.material.SetTexture("_Screen2", texture);
    }
}