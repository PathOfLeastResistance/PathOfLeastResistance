using Unity.Mathematics;

namespace ViJApps.CanvasTexture.Utils
{
    public static class MathUtils
    {
        /// <summary>
        /// Matrix to convert from 0..1 space to -1..1 space
        /// </summary>
        public static readonly float3x3 Mtr2dZeroOneToMinusOnePlusOne =
            CreateRemapMatrix2d_ZeroOne_To_MinusOnePlusOne();

        /// <summary>
        /// Matrix to convert from -1..1 space to 0..1 space
        /// </summary>
        public static readonly float3x3 Mtr2dMinusOnePlusOneToZeroOne =
            CreateRemapMatrix2d_MinusOnePlusOne_To_ZeroOne();

        /// <summary>
        /// Matrix to convert from 0..1 space to -1..1 space
        /// </summary>
        public static readonly float4x4 Mtr3dZeroOneToMinusOnePlusOne =
            CreateRemapMatrix3d_ZeroOne_To_MinusOnePlusOne();

        /// <summary>
        /// Matrix to convert from -1..1 space to 0..1 space
        /// </summary>
        public static readonly float4x4 Mtr3dMinusOnePlusOneToZeroOne =
            CreateRemapMatrix3d_MinusOnePlusOne_To_ZeroOne();

        /// <summary>
        /// 1 1 float2 vector
        /// </summary>
        public static readonly float2 Float2One = new float2(1, 1);

        /// <summary>
        /// -1 -1 float2 vector
        /// </summary>
        public static readonly float2 Float2MinusOne = new float2(-1, -1);

        public static float4 XYZ1(this float3 point) => new float4(point, 1);

        public static float4 XYZ0(this float3 point) => new float4(point, 0);

        /// <summary>
        /// Rotates vector on half pi clockwise
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float2 RotateVectorCwHalfPi(this float2 vector) => new float2(vector.y, -vector.x);

        /// <summary>
        /// Rotates vector on half pi counter clockwise
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float2 RotateVectorCcwHalfPi(this float2 vector) => new float2(-vector.y, vector.x);

        /// <summary>
        /// Converts float2 to float3
        /// </summary>
        /// <param name="val">value to be converted</param>
        /// <param name="z">default value is 0</param>
        /// <returns></returns>
        public static float3 ToFloat3(this float2 val, float z = 0) => new float3(val.x, val.y, z);

        /// <summary>
        /// Remaps value from 0..1 range to -1..1 range
        /// </summary>
        /// <param name="percentPosition"></param>
        /// <returns></returns>
        public static float2 RemapFromPercentToMinusOneOne(this float2 percentPosition) =>
            math.remap(float2.zero, Float2One, Float2MinusOne, Float2One, percentPosition);

        /// <summary>
        /// Remaps value from 0..textureSize to -1..1 range
        /// </summary>
        /// <param name="pixelPosition"></param>
        /// <param name="textureSize"></param>
        /// <returns></returns>
        public static float2 RemapFromPixelsToMinusOneOne(this float2 pixelPosition, float2 textureSize) =>
            math.remap(float2.zero, textureSize, Float2MinusOne, Float2One, pixelPosition);

        #region transformations2d

        /// <summary>
        /// Transforms point from local 2d space to world 2d space
        /// </summary>
        /// <param name="point"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float2 TransformPoint(this float2 point, float3x3 matrix) =>
            math.mul(matrix, point.ToFloat3(1)).xy;

        /// <summary>
        /// Transforms vector from local 2d space to world 2d space
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float2 TransformDirection(this float2 direction, float3x3 matrix) =>
            math.mul(matrix, direction.ToFloat3()).xy;

        /// <summary>
        /// Transforms point from world 2d space to local 2d space
        /// </summary>
        /// <param name="point"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float2 InverseTransformPoint(this float2 point, float3x3 matrix) =>
            math.mul(math.inverse(matrix), point.ToFloat3(1)).xy;

        /// <summary>
        /// Transforms vector from world 2d space to local 2d space
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static float2 InverseTransformDirection(this float2 direction, float3x3 matrix) =>
            math.mul(math.inverse(matrix), direction.ToFloat3()).xy;

        #endregion

        #region 2d matrices creation

        /// <summary>
        /// Creates remap matrix, that will remap from min..max space to -1..1 space
        /// </summary>
        /// <param name="initialMin">initial min</param>
        /// <param name="initialMax"></param>
        /// <returns></returns>
        public static float3x3 CreateRemapMatrix2d_SpaceToMinusOnePlusOne(float2 initialMin, float2 initialMax) =>
            CreateRemapMatrix2d_SpaceToSpace(initialMin, initialMax, new float2(-1, -1), new float2(1, 1));

        /// <summary>
        /// Creates remap matrix, that will remap from -1..1 space to finalMin..finalMax space
        /// </summary>
        /// <param name="finalMin"></param>
        /// <param name="finalMax"></param>
        /// <returns></returns>
        public static float3x3 CreateRemapMatrix2d_MinusOnePlusOneToSpace(float2 finalMin, float2 finalMax) =>
            CreateRemapMatrix2d_SpaceToSpace(new float2(-1, -1), new float2(1, 1), finalMin, finalMax);

        /// <summary>
        /// Creates remap matrix, that will remap from -1..1 to 0..1 space
        /// </summary>
        /// <returns></returns>
        public static float3x3 CreateRemapMatrix2d_MinusOnePlusOne_To_ZeroOne()
            => CreateRemapMatrix2d_SpaceToZeroOne(Float2MinusOne, Float2One);

        /// <summary>
        /// Creates remap matrix, that will remap from 0..1 to -1..1 space
        /// </summary>
        /// <returns></returns>
        public static float3x3 CreateRemapMatrix2d_ZeroOne_To_MinusOnePlusOne()
            => math.inverse(CreateRemapMatrix2d_MinusOnePlusOne_To_ZeroOne());

        /// <summary>
        /// Creates remap matrix, that will remap from minA..maxA to 0..1 space
        /// </summary>
        /// <param name="minA"></param>
        /// <param name="maxA"></param>
        /// <returns></returns>
        public static float3x3 CreateRemapMatrix2d_SpaceToZeroOne(float2 minA, float2 maxA)
        {
            var sizeA = maxA - minA;
            var scaleA = new float2(1, 1) / sizeA;
            var fromAto01 = math.mul(CreateMatrix2d_S(scaleA), CreateMatrix2d_T(-minA));
            return fromAto01;
        }

        /// <summary>
        /// Creates remap matrix, that will remap from 0..1 to minA..maxA space
        /// </summary>
        /// <param name="minA"></param>
        /// <param name="maxA"></param>
        /// <returns></returns>
        public static float3x3 CreateRemapMatrix2d_ZeroOneToSpace(float2 minA, float2 maxA)
            => math.inverse(CreateRemapMatrix2d_SpaceToZeroOne(minA, maxA));

        /// <summary>
        /// Creates remap matrix, that will remap from minA..maxA space to minB..maxB space
        /// </summary>
        /// <param name="minA"></param>
        /// <param name="maxA"></param>
        /// <param name="minB"></param>
        /// <param name="maxB"></param>
        /// <returns></returns>
        public static float3x3 CreateRemapMatrix2d_SpaceToSpace(float2 minA, float2 maxA, float2 minB, float2 maxB)
        {
            var sizeA = maxA - minA;
            var scaleA = new float2(1, 1) / sizeA;

            var sizeB = maxB - minB;
            var scaleB = new float2(1, 1) / sizeB;

            var fromAto01 = math.mul(CreateMatrix2d_S(scaleA), CreateMatrix2d_T(-minA));
            var fromBTo01 = math.mul(CreateMatrix2d_S(scaleB), CreateMatrix2d_T(-minB));
            var from01ToB = math.inverse(fromBTo01);
            return math.mul(from01ToB, fromAto01);
        }

        /// <summary>
        /// Creates translation matrix for 2d
        /// </summary>
        /// <param name="translation">translation vector</param>
        /// <returns></returns>
        public static float3x3 CreateMatrix2d_T(float2 translation) =>
            new float3x3(
                new float3(1, 0, 0),
                new float3(0, 1, 0),
                new float3(translation.x, translation.y, 1));

        /// <summary>
        /// Creates rotation matrix for 2d
        /// </summary>
        /// <param name="rotation">Rotation in radians</param>
        /// <returns></returns>
        public static float3x3 CreateMatrix2d_R(float rotation) =>
            new float3x3(
                new float3(math.cos(rotation), math.sin(rotation), 0),
                new float3(-math.sin(rotation), math.cos(rotation), 0),
                new float3(0, 0, 1));


        /// <summary>
        /// Creates scale matrix for 2d
        /// </summary>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static float3x3 CreateMatrix2d_S(float2 scale) =>
            new float3x3(
                new float3(scale.x, 0, 0),
                new float3(0, scale.y, 0),
                new float3(0, 0, 1));

        /// <summary>
        /// Creates translation and rotation and scale matrix for 2d
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="rotationRadians"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static float3x3 CreateMatrix2d_TRS(float2 translation, float rotationRadians, float2 scale) =>
            math.mul(CreateMatrix2d_T(translation),
                math.mul(CreateMatrix2d_R(rotationRadians), CreateMatrix2d_S(scale)));

        /// <summary>
        /// Creates translation and scale matrix for 2d
        /// </summary>
        /// <param name="translation"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static float3x3 CreateMatrix2d_TS(float2 translation, float2 scale) =>
            new float3x3(
                new float3(scale.x, 0, 0),
                new float3(0, scale.y, 0),
                new float3(translation.x, translation.y, 1));

        #endregion

        #region 3d matrices creation

        public static float4x4 CreateRemapMatrix3d_RemapSpaceToSpace(float3 minA, float3 maxA, float3 minB, float3 maxB)
        {
            var sizeA = maxA - minA;
            var scaleA = new float3(1, 1, 1) / sizeA;

            var sizeB = maxB - minB;
            var scaleB = new float3(1, 1, 1) / sizeB;

            var fromAto01 = math.mul(CreateMatrix3d_S(scaleA), CreateMatrix3d_T(-minA));
            var fromBTo01 = math.mul(CreateMatrix3d_S(scaleB), CreateMatrix3d_T(-minB));
            var from01ToB = math.inverse(fromBTo01);
            return math.mul(from01ToB, fromAto01);
        }

        public static float4x4 CreateRemapMatrix3d_MinusOnePlusOne_To_ZeroOne()
            => CreateRemapMatrix3d_SpaceToZeroOne(new float3(-1, -1, -1), new float3(1, 1, 1));

        public static float4x4 CreateRemapMatrix3d_ZeroOne_To_MinusOnePlusOne()
            => math.inverse(CreateRemapMatrix3d_MinusOnePlusOne_To_ZeroOne());

        public static float4x4 CreateRemapMatrix3d_SpaceToZeroOne(float3 minA, float3 maxA)
        {
            var sizeA = maxA - minA;
            var scaleA = new float3(1, 1, 1) / sizeA;
            var fromAto01 = math.mul(CreateMatrix3d_S(scaleA), CreateMatrix3d_T(-minA));
            return fromAto01;
        }

        public static float4x4 CreateRemapMatrix3d_SpaceToMinusOnePlusOne(float3 minA, float3 maxA)
            => CreateRemapMatrix3d_RemapSpaceToSpace(minA, maxA, new float3(-1, -1, -1), new float3(1, 1, 1));

        public static float4x4 CreateRemapMatrix3d_MinusOnePlusOneToSpace(float3 minA, float3 maxA)
            => math.inverse(CreateRemapMatrix3d_SpaceToMinusOnePlusOne(minA, maxA));

        public static float4x4 CreateRemapMatrix3d_ZeroOneToSpace(float3 minA, float3 maxA)
            => math.inverse(CreateRemapMatrix3d_SpaceToZeroOne(minA, maxA));

        public static float4x4 CreateMatrix3d_S(float3 scale) =>
            new float4x4(
                new float4(scale.x, 0, 0, 0),
                new float4(0, scale.y, 0, 0),
                new float4(0, 0, scale.z, 0),
                new float4(0, 0, 0, 1));

        public static float4x4 CreateMatrix3d_T(float3 translation) =>
            new float4x4(
                new float4(1, 0, 0, 0),
                new float4(0, 1, 0, 0),
                new float4(0, 0, 1, 0),
                new float4(translation.x, translation.y, translation.z, 1));

        public static float4x4 CreateMatrix3d_TS(float3 translation, float3 scale) =>
            new float4x4(
                new float4(scale.x, 0, 0, 0),
                new float4(0, scale.y, 0, 0),
                new float4(0, 0, scale.z, 0),
                new float4(translation.x, translation.y, translation.z, 1));

        #endregion

        #region 3d matrices operations

        public static float3 TransformPoint(this float3 point, float4x4 matrix) =>
            math.mul(matrix, new float4(point, 1)).xyz;

        public static float3 TransformDirection(this float3 direction, float4x4 matrix) =>
            math.mul(matrix, new float4(direction, 0)).xyz;

        public static float3 InverseTransformPoint(this float3 point, float4x4 matrix) =>
            math.mul(math.inverse(matrix), new float4(point, 1)).xyz;

        public static float3 InverseTransformDirection(this float3 direction, float4x4 matrix) =>
            math.mul(math.inverse(matrix), new float4(direction, 0)).xyz;

        #endregion
    }
}