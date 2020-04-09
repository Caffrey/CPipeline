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
                float3 worldPos : TEXCOORD2;
            };

                float4 unity_LightData;
                float4 unity_LightIndices[2];



            #define MAX_VISIBLE_LIGHTS 8
            float c_visible_light_count;
            half4 c_visible_light_color[MAX_VISIBLE_LIGHTS];
            half4 c_visible_light_direction[MAX_VISIBLE_LIGHTS];
            half4 c_visible_light_attenuation[MAX_VISIBLE_LIGHTS];
            half4 c_visible_light_spot_direction[MAX_VISIBLE_LIGHTS];

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;




            struct CSurfaceData
            {
                float3 Normal;
                float3 WorldPos;
                float3 Albedo;
            };
            
            struct LightData
            {
                half4 color;
                half4 directionOrPosition;
                half4 attenuation;
                half4 spotDirection;
            };

            LightData GetLight(int index)
            {
                LightData o;
                o.color = c_visible_light_color[index];
                o.directionOrPosition = c_visible_light_direction[index];
                o.attenuation = c_visible_light_attenuation[index];
                o.spotDirection = c_visible_light_spot_direction[index];

                return o;
            }

            half4 DiffuseLight(LightData light, CSurfaceData surface)
            {
                // direction light

                half4 diffuse = float4(0,0,0,1);

                //lightVector mean:
                //1.direction light direction, W = 0;
                //2.light position , W =1;
                half3 lightVector = light.directionOrPosition.xyz - surface.WorldPos * light.directionOrPosition.w;
                half3 lightDirection = normalize(lightVector);
                diffuse.rgb = max(0,dot(lightVector,surface.Normal));

                //light attenuation formual :referenece :https://catlikecoding.com/unity/tutorials/scriptable-render-pipeline/lights/
                //i / d^2 ---- i : light source intensity, d: distance 
                //point light range attenuation : (1 - (d^2/r^2)^2)^2 --- equal i from upon formual

                float distanceSqr = max(dot(lightVector, lightVector), 0.00001);

                //distance attenuation
                float rangeFade = distanceSqr * light.attenuation.x;
                rangeFade = saturate(1.0 - rangeFade*rangeFade);
                rangeFade *= rangeFade;

                //spot attenuation
                float spotFade = dot(light.spotDirection.xyz,lightDirection);
                spotFade = saturate(spotFade*light.attenuation.z + light.attenuation.w);
                spotFade *=spotFade;
                
                //point light 
                diffuse.rgb *= spotFade * rangeFade / distanceSqr;

                diffuse.rgb *= light.color.rgb; 

                return diffuse;
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
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 albedo = tex2D(_MainTex, i.uv) * _Color ;
                half4 finalColor = half4(0,0,0,1);

                fixed3 N = normalize(i.normal);


                CSurfaceData surfData;
                surfData.Normal = N;
                surfData.WorldPos = i.worldPos;
                surfData.Albedo = albedo;

                int lightLen = min(MAX_VISIBLE_LIGHTS,c_visible_light_count);
                int lightIndex = 0;
                for(int i = 0; i < lightLen; i++)
                {
                    lightIndex = GetLightIndex(i);
                    LightData lightData = GetLight(lightIndex);
                    finalColor += DiffuseLight(lightData , surfData);
                }   
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDHLSL
        }
    }
}
