using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;

public class DoubleScreenVisualizer : DisposableMonobehaviour
{
    [Header("First screen data (can be null)")]
    [SerializeField] private OscilloscopeScreen m_screen1;
    [Header("Second screen data (can be null")]
    [SerializeField] private OscilloscopeScreen m_screen2;
    
    [Header("Screen settings")]
    [SerializeField] private OscilloscopeScreenText m_screenTexts;
    [SerializeField] private Renderer m_renderer;

    [SerializeField] private Vector2 m_voltageRange = new Vector2(-5, 5);
    [SerializeField] private float m_VolstPerCell = 1f;
    
    private bool m_screen1Active = true;
    private bool m_screen2Active = true;

    private void Start()
    {
        if (m_screen1 != null)
        {
            SetFirstTexture(m_screen1.RenderTexture);
            m_screen1.SubscribeRenderTexture(SetFirstTexture).DisposeWhenNotifierDisposed(this);
        }
        else
        {
            Debug.LogWarning("Screen 1 is null");
        }

        if (m_screen2 != null)
        {
            SetSecondTexture(m_screen2.RenderTexture);
            m_screen2.SubscribeRenderTexture(SetSecondTexture).DisposeWhenNotifierDisposed(this);
        }
        else
        {
            Debug.LogWarning("Screen 2 is null");
        }

        m_screenTexts.SubscribeRenderTexture(SetTextTexture).DisposeWhenNotifierDisposed(this);
        UpdateScreenSettings();
    }

    private void UpdateScreenSettings()
    {
        //Init the material for the grid
        var mat = m_renderer.material;
        mat.SetFloat("_Intensity1", m_screen1Active ? 1 : 0);
        mat.SetFloat("_Intensity2", m_screen2Active ? 1 : 0);

        var zeroPos = math.remap(m_voltageRange.x, m_voltageRange.y, 0, 1, 0);
        mat.SetFloat("_ZeroVerticalOffset", zeroPos);

        var verticalCellCount = (m_voltageRange.y - m_voltageRange.x) / m_VolstPerCell;
        mat.SetVector("_CellCount", new Vector4(5, verticalCellCount, 0, 0));

        //Initialize voltage data visualization
        //TODO: hardcode for now. it can be configurable for real oscilloscopes
        var settings = new OscilloscopeDataSettings()
        {
            MinVoltage = m_voltageRange.x,
            MaxVoltage = m_voltageRange.y,
            RenderPeriod = 0.5f, 
        };

        if (m_screen1 != null)
        {
            m_screen1.Settings = settings;
            m_screen1.DrawScanner = true;
            m_screen1.DrawText = true;
        }

        if (m_screen2 != null)
        {
            m_screen2.Settings = settings;
            m_screen2.DrawScanner = false;
            m_screen2.DrawText = false;
        }

        //Initialize voltage text renderers
        m_screenTexts.m_voltagePerUnit = m_VolstPerCell * 2; //Skip every second line. TODO: make it configurable
        m_screenTexts.VoltageRange = m_voltageRange;
    }
    
    private void SetTextTexture(RenderTexture texture) =>m_renderer.material.SetTexture("_TextTexture", texture);

    private void SetSecondTexture(RenderTexture texture) => m_renderer.material.SetTexture("_Screen2", texture);

    private void SetFirstTexture(RenderTexture texture) => m_renderer.material.SetTexture("_Screen1", texture);
}