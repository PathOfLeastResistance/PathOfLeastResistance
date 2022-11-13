using System;
using TMPro;
using UnityEngine;

namespace ViJApps.CanvasTexture
{
    [Serializable]
    public struct SpacingOptions
    {
        public float Word;
        public float Character;
        public float Line;
        public float Paragraph;
    }
    
    [Serializable]
    public struct TextSettings
    {
        public float FontSize;
        public Color FontColor;
        public FontStyles FontStyle;
        public FontWeight FontWeight;
        public TextAlignmentOptions TextAlignment;

        public SpacingOptions SpacingOptions;

        public bool Wrapping;
        public TextOverflowModes OverflowMode;

        public static TextSettings Default =>
            new TextSettings
            {
                FontSize = 1,
                FontColor = Color.black,
                FontStyle = FontStyles.Normal,
                FontWeight = FontWeight.Regular,
                TextAlignment = TextAlignmentOptions.TopLeft,
                
                SpacingOptions = new SpacingOptions(),
                
                Wrapping = true,
                OverflowMode = TextOverflowModes.Overflow,
            };
    }
}