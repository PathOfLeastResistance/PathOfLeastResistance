using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DoubleScreenVisualizer : MonoBehaviour
{
    [SerializeField] private OscilloscopeScreen m_screen1;
    [SerializeField] private OscilloscopeScreen m_screen2;

    [SerializeField] private Renderer m_renderer;

    private bool m_screen1Active = true;
    private bool m_screen2Active = true;

    private Vector2 m_voltageRange = new Vector2(-5, 5);

    private float m_VolstPerCell = 1f;

    public bool Screen1Active
    {
        get => m_screen1Active;
        set
        {
            m_screen1Active = value;
            RefreshScreenMaterial();
        }
    }

    public bool Screen2Active
    {
        get => m_screen2Active;
        set
        {
            m_screen2Active = value;
            RefreshScreenMaterial();
        }
    }

    public Vector2 VotageRange
    {
        get => m_voltageRange;
        set
        {
            m_voltageRange = value;
            RefreshScreenMaterial();
        }
    }

    public float VoltsPerCell
    {
        get =>  m_VolstPerCell;
        set
        {
            m_VolstPerCell = value;
            RefreshScreenMaterial();
        }
    }

    private void Awake()
    {
        if (m_screen1 != null)
        {
            SetFirstTexture(m_screen1.RenderTexture);
            m_screen1.OnRenderTextureChanged += SetFirstTexture;
        }
        else
        {
            Debug.LogWarning("Screen 1 is null");
        }
        
        if (m_screen2 != null)
        {
            SetSecondTexture(m_screen2.RenderTexture);
            m_screen2.OnRenderTextureChanged += SetSecondTexture;
        }
        else
        {
            Debug.LogWarning("Screen 2 is null");
        }

        RefreshScreenMaterial();
    }

    private void RefreshScreenMaterial()
    {
        var mat = m_renderer.material;
        mat.SetFloat("_Intensity1", m_screen1Active ? 1 : 0);
        mat.SetFloat("_Intensity2", m_screen2Active ? 1 : 0);

        var zeroPos = math.remap(m_voltageRange.x, m_voltageRange.y, 0, 1, 0);
        mat.SetFloat("_ZeroVerticalOffset", zeroPos);
        
        var verticalCellCount = (m_voltageRange.y - m_voltageRange.x) / m_VolstPerCell;
        mat.SetVector("_CellCount", new Vector4(5, verticalCellCount, 0, 0));
    }

    private void SetSecondTexture(RenderTexture texture) =>     m_renderer.material.SetTexture("_Screen2", texture);
    
    private void SetFirstTexture(RenderTexture texture) =>      m_renderer.material.SetTexture("_Screen1", texture);
}