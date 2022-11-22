using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using ViJApps.CanvasTexture.Utils;
using MathUtils = ViJApps.CanvasTexture.Utils.MathUtils;

namespace ViJApps.CanvasTexture
{
    public enum SimpleLineEndingStyle
    {
        None = 0,
        Round = 1,
    }

    public partial class CanvasTexture : IDisposable
    {
        //Data
        private CommandBuffer m_cmd;
        private RenderTextureDescriptor m_textureDescriptor;

        //Pools
        private readonly MeshPool m_meshPool = new();
        private readonly List<Mesh> m_allocatedMeshes = new();
        private readonly PropertyBlockPool m_propertyBlockPool = new();
        private readonly List<MaterialPropertyBlock> m_allocatedPropertyBlocks = new();
        private readonly TextComponentsPool m_textComponentsPool = new();
        private readonly List<TextComponent> m_allocatedTextComponents = new();

        //coord systems used for painting
        private LinearCoordSystem m_textureCoordSystem;

        public readonly AspectSettings AspectSettings = new();

        public RenderTexture RenderTexture { get; private set; }

        public void Init(RenderTextureDescriptor descriptor)
        {
            m_textureDescriptor = descriptor;
            if (RenderTexture != null)
                UnityEngine.Object.Destroy(RenderTexture);
            RenderTexture = new RenderTexture(m_textureDescriptor);
            m_textureCoordSystem = new LinearCoordSystem(new float2(m_textureDescriptor.width, m_textureDescriptor.height));
            
            ResetCmd();
            ReleaseToPools();
        }
        
        /// <summary>
        /// Initialize CanvasTexture with given
        /// </summary>
        /// <param name="renderTexture"></param>
        public void Init(RenderTexture renderTexture)
        {
            m_textureDescriptor = renderTexture.descriptor;
            RenderTexture = renderTexture;
            m_textureCoordSystem = new LinearCoordSystem(new float2(renderTexture.width, renderTexture.height));

            ResetCmd();
            ReleaseToPools();
        }

        public void Init(int size) => Init(size, size);

        /// <summary>
        /// Init canvas with size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Init(int width, int height)
        {
            if (RenderTexture == null || RenderTexture.width != width || RenderTexture.height != height)
                ReinitTexture(width, height);

            ResetCmd();
            ReleaseToPools();
        }

        /// <summary>
        /// Executes all commands and clears command buffer
        /// </summary>
        public void Flush()
        {
            Graphics.ExecuteCommandBuffer(m_cmd);
           
            ResetCmd();
            ReleaseToPools();
        }

        /// <summary>
        /// Clears texture with given color
        /// </summary>
        /// <param name="color"></param>
        public void ClearWithColor(Color color) => m_cmd.ClearRenderTarget(RTClearFlags.All, color, 1f, 0);

        public Texture2D ToTexture2D(Texture2D texture = null)
        {
            if (texture == null)
                texture = new Texture2D(RenderTexture.width, RenderTexture.height);
            else
                texture.Reinitialize(RenderTexture.width, RenderTexture.height);

            var buffer = RenderTexture.active;
            RenderTexture.active = RenderTexture;
            texture.ReadPixels(new Rect(0, 0, RenderTexture.width, RenderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = buffer;
            return texture;
        }

        public void SaveToAssets(string assetName)
        {
#if UNITY_EDITOR
            var texture2d = ToTexture2D();
            var bytes = texture2d.EncodeToPNG();
            System.IO.File.WriteAllBytes($"Assets/" + assetName + ".png", bytes);
            AssetDatabase.Refresh();
#else
            Debug.LogWarning("You can save to assets only from Editor");
#endif
        }

        public void Dispose()
        {
            ReleaseToPools();
            m_meshPool.Clear();
            m_propertyBlockPool.Clear();
            m_textComponentsPool.Clear();

            if (m_cmd != null)
            {
                m_cmd.Dispose();
                m_cmd = null;
            }

            if (RenderTexture != null)
            {
                UnityEngine.Object.Destroy(RenderTexture);
                RenderTexture = null;
            }
        }

        private (Mesh mesh, MaterialPropertyBlock block) AllocateMeshAndPropertyBlock()
        {
            var mesh = m_meshPool.Get();
            m_allocatedMeshes.Add(mesh);

            var propertyBlock = m_propertyBlockPool.Get();
            m_allocatedPropertyBlocks.Add(propertyBlock);
            return (mesh, propertyBlock);
        }

        private (float positiveOffset, float negativeOffset) GetOffsets(float strokeOffset, float strokeThickness)
        {
            strokeOffset = math.clamp(strokeOffset, 0, 1);
            return (strokeThickness * strokeOffset, -strokeThickness * (1f - strokeOffset));
        }

        private void ResetCmd()
        {
            if (m_cmd == null)
                m_cmd = new CommandBuffer();
            else
                m_cmd.Clear();
            m_cmd.SetViewProjectionMatrices(Utils.MathUtils.Mtr3dZeroOneToMinusOnePlusOne, Matrix4x4.identity);
            m_cmd.SetRenderTarget(RenderTexture);
        }

        private void ReinitTexture(int width, int height)
        {
            if (RenderTexture == null)
            {
                CreateRenderTexture(width, height);
            }
            else if (RenderTexture.width != width || RenderTexture.height != height)
            {
                UnityEngine.Object.Destroy(RenderTexture);
                CreateRenderTexture(width, height);
            }
        }

        private void CreateRenderTexture(int width, int height)
        {
            m_textureDescriptor = new RenderTextureDescriptor(width, height);
            RenderTexture = new RenderTexture(m_textureDescriptor);
            m_textureCoordSystem = new LinearCoordSystem(new float2(width, height));
        }

        private void ReleaseToPools()
        {
            foreach (var mesh in m_allocatedMeshes)
                m_meshPool.Release(mesh);
            m_allocatedMeshes.Clear();

            foreach (var block in m_allocatedPropertyBlocks)
                m_propertyBlockPool.Release(block);
            m_allocatedPropertyBlocks.Clear();

            foreach (var text in m_allocatedTextComponents)
                m_textComponentsPool.Release(text);
            m_allocatedTextComponents.Clear();
        }
    }
}