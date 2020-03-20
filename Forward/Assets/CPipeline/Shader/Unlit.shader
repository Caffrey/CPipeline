Shader "CPipeLine/Lit"
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
            float c_visible_light_count;
            half4 c_visible_light_color[MAX_VISIBLE_LIGHTS];
            half4 c_visible_light_direction[MAX_VISIBLE_LIGHTS];


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;




            
            struct LightData
            {
                half4 color;
                half4 direction;
            };

            LightData GetLight(int index)
            {
                LightData o;
                o.color = c_visible_light_color[index];
                o.direction = c_visible_light_direction[index];
                return o;
            }

            int GetLightIndex(int index)
            {
                return unity_LightIndices[index/4][index%4];
            }


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
                fixed4 albedo = tex2D(_MainTex, i.uv) * _Color ;
                half4 finalColor = half4(0,0,0,1);

                fixed3 N = normalize(i.normal);

                int lightLen = min(MAX_VISIBLE_LIGHTS,c_visible_light_count);
                int lightIndex = 0;
                for(int i = 0; i < lightLen; i++)
                {
                    lightIndex = GetLightIndex(i);
                    LightData lightData = GetLight(lightIndex);
                    finalColor += max(0,dot(lightData.direction,N)) * lightData.color * albedo;
                }   
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDHLSL
        }
    }
}
