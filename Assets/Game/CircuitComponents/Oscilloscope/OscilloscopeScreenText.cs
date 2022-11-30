using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityTools;
using ViJApps.CanvasTexture;

public class OscilloscopeScreenText : MonoBehaviour
{
    public float m_voltagePerUnit = 1f;

    private CanvasTexture m_canvasTexture;
    private event Action<RenderTexture> OnRenderTextureChanged;

    [SerializeField] private bool m_isDirty = true;
    [SerializeField] private TextSettings m_textSettings;
    [SerializeField] private Vector2 m_voltageRange = new Vector2(-5, 5);

    //Subscribe rendet texture changed
    [SerializeField] private RenderTexture m_renderTexture;

    public RenderTexture RenderTexture
    {
        get => m_renderTexture;
        private set
        {
            m_renderTexture = value;
            OnRenderTextureChanged?.Invoke(m_renderTexture);
        }
    }

    public Vector2 VoltageRange
    {
        get => m_voltageRange;
        set
        {
            m_voltageRange = value;
            m_isDirty = true;
        }
    }

    public float VoltagePerUnit
    {
        get => m_voltagePerUnit;
        set
        {
            m_voltagePerUnit = value;
            m_isDirty = true;
        }
    }

    public IDisposable SubscribeRenderTexture(Action<RenderTexture> onRenderTextureChanged)
    {
        OnRenderTextureChanged += onRenderTextureChanged;
        onRenderTextureChanged?.Invoke(m_renderTexture);
        return new DisposableAction(() => OnRenderTextureChanged -= onRenderTextureChanged);
    }

    private async void Start()
    {
        m_canvasTexture = new CanvasTexture();
        m_canvasTexture.Init(OscilloscopeScreen.ScreenResolutionX * 2, OscilloscopeScreen.ScreenResolutionY * 2);
        RenderTexture = m_canvasTexture.RenderTexture;
        RenderTexture.autoGenerateMips = true;
    }

    private void Update()
    {
        //Temporal peace of sheet
        if (m_isDirty)
        {
            m_canvasTexture.ClearWithColor(Color.black);

            var currentVoltage = 0.0f;
            var step = m_voltagePerUnit;
            var counter = 0;

            while (currentVoltage < m_voltageRange.y)
            {
                currentVoltage = counter * step;
                var posY = math.remap(m_voltageRange.x, m_voltageRange.y, 0, 1, currentVoltage);
                m_canvasTexture.DrawText($"{currentVoltage}", m_textSettings, new Vector2(0.1f, posY), Vector2.one,0, Vector2.zero);
                counter++;
            }

            counter = 1;
            currentVoltage = -step * counter;
            while(currentVoltage > m_voltageRange.x)
            {
                var posY = math.remap(m_voltageRange.x, m_voltageRange.y, 0, 1, currentVoltage);
                m_canvasTexture.DrawText($"{currentVoltage}", m_textSettings, new Vector2(0.1f, posY), Vector2.one,0, Vector2.zero);
                counter++;
                currentVoltage = -step * counter;
            }
            
            m_canvasTexture.Flush();

            m_isDirty = false;
        }
    }
}