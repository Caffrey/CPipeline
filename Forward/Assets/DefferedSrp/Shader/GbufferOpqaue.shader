Shader "Unlit/GbufferOpaque"
{
    Properties
    {
        _Color("Color",Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
    }

    HLSLINCLUDE
    #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            float3 normal : NORMAL;
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
             float4 vertex : SV_POSITION;
             float3 normal :TEXCOORD1;
        };

        struct RTStruct
        {
            float4 albedo:SV_Target0;
            float4 normal:SV_Target1;

        };

        sampler2D _MainTex;
        float4 _MainTex_ST;
        half4 _Color;

        v2f vert(appdata v)
        {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.normal = UnityObjectToWorldNormal(v.normal);
            return o;
        }

        RTStruct frag(v2f i) 
        {
            RTStruct o;
            o.albedo = tex2D(_MainTex, i.uv) * _Color;
            o.normal = half4(i.normal, 0);
            return o;
        }


    ENDHLSL


    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="GBuffer"}


        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag           
            ENDHLSL
        }
    }
}
