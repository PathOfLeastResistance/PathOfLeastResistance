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

    private int m_pixelsCouunt = 1000;

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
        var pixels = new List<int2>(m_pixelsCouunt);
        for (int i = 0; i< m_pixelsCouunt; i++)
            pixels[i] = new int2(UnityEngine.Random.Range(0,32), UnityEngine.Random.Range(0,32));
        
        m_canvasTexture.DrawPixels(pixels, Color.red);
        m_canvasTexture.Flush();
    }
}
