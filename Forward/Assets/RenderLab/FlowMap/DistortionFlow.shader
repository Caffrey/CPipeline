Shader "Custom/DistortionFlow" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [NoScaleOffset] _FlowMap ("Flow (RG)", 2D) = "black" {}
        [NoScaleOffset] _NormalMap ("Normals", 2D) = "bump" {}
        _Tiling ("Tiling", Float) = 1
        _FlowStrength ("Flow Strength", Float) = 1
        _UJump ("U jump per phase", Range(-0.25, 0.25)) = 0.25
        _VJump ("V jump per phase", Range(-0.25, 0.25)) = 0.25
        _FlowOffset ("Flow Offset", Float) = 0
        _Speed ("Speed",Float) = 1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        #include "Flow.cginc"

        sampler2D _MainTex,_FlowMap,_NormalMap;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _UJump, _VJump, _Tiling,_Speed,_FlowStrength,_FlowOffset;
        void surf (Input IN, inout SurfaceOutputStandard o) {
            float4 flowData = tex2D(_FlowMap,IN.uv_MainTex);
            flowData.xy = flowData.xy * 2 - 1;
            float time = _Time.y + flowData.a;
            time *= _Speed;

            float2 jump = float2(_UJump, _VJump);
            float flowVector = flowData.xy * _FlowStrength;

            // flowVector = 0;
            float3 uvwA = FlowUVW(IN.uv_MainTex, flowVector,_Tiling, jump,_FlowOffset, time, false);
            float3 uvwB = FlowUVW(IN.uv_MainTex, flowVector,_Tiling, jump,_FlowOffset,time, true);

            fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
            fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;
            float3 normalA = UnpackNormal(tex2D(_NormalMap, uvwA.xy)) * uvwA.z;
            float3 normalB = UnpackNormal(tex2D(_NormalMap, uvwB.xy)) * uvwB.z;
            o.Normal = normalize(normalA + normalB);

            fixed4 c = (texA + texB) * _Color;


            o.Albedo = c.rgb ;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}