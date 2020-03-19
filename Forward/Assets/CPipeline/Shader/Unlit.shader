﻿Shader "Unlit/Unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode" = "SRPDefaultUnlits" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

                float4 unity_LightData;
                float4 unity_LightIndices[2];



#define MAX_VISIBLE_LIGHTS 8
            float _lightLength;
            float4 _lightColors[MAX_VISIBLE_LIGHTS];
            float4 _lightDirection[MAX_VISIBLE_LIGHTS];

            float4 unity_PerObjectLightData;
            float4 unity_PerObjectLightIndices[2];

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color ;
            col *= _lightColors[0] * _lightDirection[0];
            col *= _lightColors[1] * _lightDirection[1];
            col *= _lightColors[2] * _lightDirection[2];
            col.r += _lightLength;
                // apply fog
            col.r += unity_LightData.x;
            col.g += unity_LightIndices[0].x;
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
    }
}
