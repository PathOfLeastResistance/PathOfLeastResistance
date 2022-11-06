using Unity.Mathematics;

namespace ViJApps.CanvasTexture
{
    public class AspectSettings
    {
        private float m_aspect = 1f;
        
        /// <summary>
        /// Aspect interpretation
        /// </summary>
        public float Aspect
        {
            get => m_aspect;
            set
            {
                m_aspect = math.max(value, 0.0001f);
                AspectMatrix2d = Utils.MathUtils.CreateMatrix2d_S(new float2(m_aspect, 1));
                InverseAspectMatrix2d = math.inverse(AspectMatrix2d);
                
                AspectMatrix3d = Utils.MathUtils.CreateMatrix3d_S(new float3(m_aspect, 1, 1));
                InverseAspectMatrix3d = math.inverse(AspectMatrix3d);
            }
        }

        public float3x3 AspectMatrix2d { get; private set; } = float3x3.identity;
        public float3x3 InverseAspectMatrix2d { get; private set; } = float3x3.identity;
        public float4x4 AspectMatrix3d{ get; private set; } = float4x4.identity;
        public float4x4 InverseAspectMatrix3d{ get; private set; } = float4x4.identity;
    }
}