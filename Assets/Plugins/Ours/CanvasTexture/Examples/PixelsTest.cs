using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using ViJApps.CanvasTexture;

public class PixelsTest : MonoBehaviour
{
    [SerializeField] private Renderer m_renderer;

    private CanvasTexture m_canvasTexture;
    
    [SerializeField] private int2[] m_pixels = new int2[0];

    private async void Awake()
    {
        await MaterialProvider.Initialization;
        m_canvasTexture = new CanvasTexture();
        m_canvasTexture.Init(32,32);
        m_renderer.material.mainTexture = m_canvasTexture.RenderTexture;
    }

    private async void Update()
    {
        await MaterialProvider.Initialization;

        m_canvasTexture.ClearWithColor(Color.black);
        m_canvasTexture.DrawPixels(m_pixels, Color.red);
        m_canvasTexture.Flush();
    }
}
