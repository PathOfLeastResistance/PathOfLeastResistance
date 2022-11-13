using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace ViJApps.CanvasTexture
{
    public partial class CanvasTexture 
    {

        public void DrawText(string text, TextSettings textSettings, float2 position, float rotation = 0) => DrawText(text, textSettings, position, sizeDelta: new float2(1, 1), rotation: rotation);

        public void DrawText(string text, TextSettings textSettings, float2 position, float2 sizeDelta, float rotation = 0) =>
            DrawText(text, textSettings, position, sizeDelta, rotation, pivot: new float2(0.5f, 0.5f));

        public void DrawText(string text, TextSettings textSettings, float2 position, float2 sizeDelta, float rotation, float2 pivot)
        {
            //Prepare text mesh
            var textComponent = m_textComponentsPool.Get();
            m_allocatedTextComponents.Add(textComponent);
            textComponent.Text = text;

            //Position and size
            textComponent.Pivot = pivot;
            textComponent.Position = position;
            textComponent.SizeDelta = sizeDelta;
            textComponent.Rotation = rotation;
            textComponent.Aspect = AspectSettings.Aspect;

            //Set settings, update mesh and add to render
            textComponent.SetSettings(textSettings);
            textComponent.UpdateText();
            m_cmd.DrawRenderer(textComponent.Renderer, textComponent.Material);
        }
    }
}
