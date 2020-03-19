Shader "Unlit/Unlit"
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
                float3 normal: NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

                float4 unity_LightData;
                float4 unity_LightIndices[2];



            #define MAX_VISIBLE_LIGHTS 8
            float _lightLength;
            float4 _lightColors[MAX_VISIBLE_LIGHTS];
            float4 _lightDirection[MAX_VISIBLE_LIGHTS];


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex); 
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color ;
                col *= max(0, dot(normalize(i.normal), _lightDirection[0])) * _lightColors[0];
                col += max(0, dot(normalize(i.normal), _lightDirection[1])) * _lightColors[1];
                col += max(0, dot(normalize(i.normal), _lightDirection[2])) * _lightColors[2];
                col += max(0, dot(normalize(i.normal), _lightDirection[3])) * _lightColors[3];
                col += max(0, dot(normalize(i.normal), _lightDirection[4])) * _lightColors[4];
                col.r += unity_LightData.x;
                col.g += unity_LightIndices[0].x;

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
    }
}
