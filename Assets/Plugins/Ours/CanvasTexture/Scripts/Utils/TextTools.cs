using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ViJApps.CanvasTexture.Utils
{
    public static class TextTools
    {
        public static float GetWorldSize(this TMP_FontAsset fontAsset, float fontSize) =>
            fontAsset.GetWorldSizeForOnePoint() * fontSize;

        public static float GetFontSizeToFitWorldSize(this TMP_FontAsset fontAsset, float worldSize) =>
            worldSize / fontAsset.GetWorldSizeForOnePoint();

        public static float GetWorldSizeForOnePoint(this TMP_FontAsset fontAsset)
        {
            var faceInfo = fontAsset.faceInfo;
            var pointSize = faceInfo.pointSize;
            var ascentLine = faceInfo.ascentLine;
            var descentLine = faceInfo.descentLine;
            var delta = (ascentLine - descentLine) / 10f;
            var scale = delta / pointSize;
            return scale;
        }
    }
}