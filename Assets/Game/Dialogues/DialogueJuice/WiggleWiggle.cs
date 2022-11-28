using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct WiggleContainer
{
    public RectTransform RectTransform;
    public float Amplitude;
}

public class WiggleWiggle : MonoBehaviour
{
    [SerializeField] private List<WiggleContainer> m_rectsToWiggle;

    private List<Vector3> m_originalPositions;

    private float m_t1 = 10;
    private float m_t2 = 100;

    [SerializeField] private float m_speed = 0.1f;

    private void Awake()
    {
        m_originalPositions = new List<Vector3>(m_rectsToWiggle.Count);
        foreach (var rect in m_rectsToWiggle)
            m_originalPositions.Add(rect.RectTransform.localPosition);
    }

    private void UpdatePos()
    {
        var x = Mathf.PerlinNoise1D((Time.time + m_t1) * m_speed);
        var y = Mathf.PerlinNoise1D((Time.time + m_t2) * m_speed);

        for (int i = 0; i < m_rectsToWiggle.Count; i++)
        {
            var rect = m_rectsToWiggle[i];
            var originalPos = m_originalPositions[i];
            rect.RectTransform.localPosition = originalPos + new Vector3(x * rect.Amplitude, y * rect.Amplitude, 0);
        }
    }

    private void Update()
    {
        UpdatePos();
    }
}