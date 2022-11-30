Shader "ViJApps.SimpleLineUnlit"
{
    Properties
    {
        _FromToCoord("_FromToCoord", Vector) = (0, 0, 0, 0)
        _Thickness("_Thickness", Float) = 0
        _Color ("_Color", Color) = (0, 0, 0, 1)

        _Aspect("_Aspect", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
        }
        Tags
        {
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Always
        ZWrite Off

        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "math.hlsl"

            //Figure data
            uniform half4 _Color;
            uniform half _Thickness;
            uniform half4 _FromToCoord;

            //Aspect data
            uniform half _Aspect;
            uniform half3x3 _InverseAspectMatrix;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 localPos: COLOR0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.localPos = v.vertex.xyz;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                _InverseAspectMatrix = InverseScaleMatrixFromAspect(_Aspect);

                float2 p = TransformPoint(_InverseAspectMatrix, i.localPos.xy);
                float2 from = TransformPoint(_InverseAspectMatrix, _FromToCoord.xy);
                float2 to = TransformPoint(_InverseAspectMatrix, _FromToCoord.zw);

                float distance = sdLineSegment(p, from, to);
                float isLine = step(distance, _Thickness / 2);
                return lerp(float4(0, 0, 0, 0), _Color, isLine);
            }
            ENDHLSL
        }
    }
}