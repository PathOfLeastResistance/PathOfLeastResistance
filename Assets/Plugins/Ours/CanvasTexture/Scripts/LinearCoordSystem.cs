using Unity.Mathematics;
using ViJApps.CanvasTexture.Utils;

namespace ViJApps.CanvasTexture
{
    /// <summary>
    /// This class helps to convert from given space to 0..1 and -1..1 spaces 
    /// </summary>
    public class LinearCoordSystem
    {
        //2d conversion matrices
        public float3x3 WorldToZeroOne2d { get; private set; }
        public float3x3 ZeroOneToWorld2d { get; private set; }
        public float3x3 WorldToMinusOnePlusOne2d { get; private set; }
        public float3x3 MinusOnePlusOneToWorld2d { get; private set; }

        //3d conversion matrices
        public float4x4 WorldToZeroOne3d { get; private set; }
        public float4x4 ZeroOneToWorld3d { get; private set; }
        public float4x4 WorldToMinusOnePlusOne3d { get; private set; }
        public float4x4 MinusOnePlusOneToWorld3d { get; private set; }

        public float2 From { get; private set; }

        public float2 To { get; private set; }

        public float2 Size => math.abs(To - From);

        public float Width => Size.x;

        public float Height => Size.y;

        public LinearCoordSystem(float2 size) : this(float2.zero, size)
        {
        }

        public LinearCoordSystem(float2 worldFrom, float2 worldTo)
        {
            InitCoordSystem(worldFrom, worldTo);
        }

        public void InitCoordSystem(float2 size) => InitCoordSystem(float2.zero, size);

        public void InitCoordSystem(float2 worldFrom, float2 worldTo)
        {
            From = worldFrom;
            To = worldTo;

            //0..1 2d
            WorldToZeroOne2d = MathUtils.CreateRemapMatrix2d_SpaceToZeroOne(From, To);
            ZeroOneToWorld2d = MathUtils.CreateRemapMatrix2d_ZeroOneToSpace(From, To);

            //-1..1 2d
            WorldToMinusOnePlusOne2d = MathUtils.CreateRemapMatrix2d_SpaceToMinusOnePlusOne(From, To);
            MinusOnePlusOneToWorld2d = MathUtils.CreateRemapMatrix2d_MinusOnePlusOneToSpace(From, To);

            //0..1 3d
            WorldToZeroOne3d =
                MathUtils.CreateRemapMatrix3d_SpaceToZeroOne(From.ToFloat3(0), To.ToFloat3(1));
            ZeroOneToWorld3d =
                MathUtils.CreateRemapMatrix3d_ZeroOneToSpace(From.ToFloat3(0), To.ToFloat3(1));

            //-1..1 3d
            WorldToMinusOnePlusOne3d =
                MathUtils.CreateRemapMatrix3d_SpaceToMinusOnePlusOne(From.ToFloat3(0), To.ToFloat3(1));
            MinusOnePlusOneToWorld3d =
                MathUtils.CreateRemapMatrix3d_MinusOnePlusOneToSpace(From.ToFloat3(0), To.ToFloat3(1));
        }
    }
}