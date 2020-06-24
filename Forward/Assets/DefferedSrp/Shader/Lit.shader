﻿Shader "Unlit/Lit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    HLSLINCLUDE
    #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
             float4 vertex : SV_POSITION;
        };

        sampler2D _MainTex;
        float4 _MainTex_ST;

        float4 gBufferAlbedo:SV_Target0;
        float4 gBufferNormal:SV_Target1;

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            fixed4 col = tex2D(_MainTex, i.uv);
        return col;
        }


    ENDHLSL


    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="Opaque"}


        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag           
            ENDHLSL
        }
    }
}
