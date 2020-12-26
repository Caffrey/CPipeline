Shader "CRenderLab/BasicPBR"
{
    Properties
    {
       	_AlbedoTex("Base Color",2D) = "white"{}
       	[NoScaleOffset] 
       	_NormalTex("Normal Map",2D) =  "bump" {}
       	[NoScaleOffset] 
       	_RoughnessTex("Roughness Map",2D) = "white"{}
       	[NoScaleOffset] 
       	_MetallicTex("Metallic Map",2D) = "white"{}
       	[NoScaleOffset] 
       	_AOTex("Ambient Oclussion Map",2D) = "white"{}


       	_Tint ("Tint", Color) = (1,1,1,1)
   		_Metallic("Metallic",Range(0,1)) = 1
   		_Roughness("Roughness",Range(0,1)) = 1
    }


    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100


        CGINCLUDE

            #include "UnityCG.cginc"
	        #include "AutoLight.cginc"
	        #include "Lighting.cginc"

 			struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 lightmap_uv:TEXCOORD1;

                float4 tangent : TANGENT;
    			float3 normal : NORMAL;
            };

            struct v2f
            {
               
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 lightmap_uv : TEXCOORD1;
                float4 w2t0: TEXCOORD2; 
                float4 w2t1: TEXCOORD3;
                float4 w2t2: TEXCOORD4;
                SHADOW_COORDS(5)

            };
 
            sampler2D _AlbedoTex,_NormalTex,_RoughnessTex,_MetallicTex,_AOTex;
            float4 _AlbedoTex_ST,_Tint;

             float _Metallic,_Roughness;
            //----------------------PBR
            // Reference :https://learnopengl.com/PBR/Lighting
            // PBR Function Reference : https://www.jordanstevenstechart.com/physically-based-rendering


            #define PI 3.14159265359


            float3 fresnelSchlick(float HdotV, float3 F0)
			{
			    return F0 + (1.0 - F0) * pow(max(1.0 - HdotV, 0.0), 5.0);
			}  	


			float DistributionGGX(float3 N, float3 H, float roughness)
			{
			    float a      = roughness*roughness;
			    float a2     = a*a;
			    float NdotH  = max(dot(N, H), 0.0);
			    float NdotH2 = NdotH*NdotH;
				
			    float num   = a2;
			    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
			    denom = PI * denom * denom;
				
			    return num / denom;
			}

			float GeometrySchlickGGX(float NdotV, float roughness)
			{
			    float r = (roughness + 1.0);
			    float k = (r*r) / 8.0;

			    float num   = NdotV;
			    float denom = NdotV * (1.0 - k) + k;
				
			    return num / denom;
			}
			float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
			{
			    float NdotV = max(dot(N, V), 0.0);
			    float NdotL = max(dot(N, L), 0.0);
			    float ggx2  = GeometrySchlickGGX(NdotV, roughness);
			    float ggx1  = GeometrySchlickGGX(NdotL, roughness);
				
			    return ggx1 * ggx2;
			}

            //----------------------PBR

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _AlbedoTex);
                o.lightmap_uv = v.lightmap_uv;

                float3 world_pos = mul(unity_ObjectToWorld,v.vertex).xyz;
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);  
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);  
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w; 

    			o.w2t0 = float4(worldTangent.x,worldBinormal.x, worldNormal.x, world_pos.x);
    			o.w2t1 = float4(worldTangent.y,worldBinormal.y, worldNormal.y, world_pos.y);
    			o.w2t2 = float4(worldTangent.z,worldBinormal.z, worldNormal.z, world_pos.z);

    			TRANSFER_SHADOW(o);
               return o;
            }

            float3 PBRLighting(v2f i)
            {
            	float3 WPOS = float3(i.w2t0.w, i.w2t1.w, i.w2t2.w);

            	float4 Albedo = tex2D(_AlbedoTex,i.uv) * _Tint;

            	float3 WN = UnpackNormal(tex2D(_NormalTex,i.uv));
            	WN.z = sqrt(1.0 - saturate(dot(WN.xy, WN.xy)));
            	WN.xyz = normalize(float3(
            		dot(i.w2t0.xyz,WN),
            		dot(i.w2t1.xyz,WN),
            		dot(i.w2t2.xyz,WN)
            		));
            	
            	float Roughness = tex2D(_RoughnessTex,i.uv).r * _Roughness;
            	float Metallic = tex2D(_MetallicTex,i.uv).r * _Metallic;
            	float AO = tex2D(_AOTex,i.uv).r;


            	float3 WL = normalize(UnityWorldSpaceLightDir(WPOS));
            	float3 WV = normalize(UnityWorldSpaceViewDir(WPOS));
            	float3 WH = normalize(WL + WV);

            	//Basic Setup
            	float NdotL = saturate(dot(WN,WL));
            	float NdotV = saturate(dot(WN,WV));
            	float HdotV = saturate(dot(WH,WV));

            	//Specular
            	float3 F0 = float3(0.4,0.4,0.4);
            	F0 = lerp(F0,Albedo,Metallic);
            	float3 F = fresnelSchlick(HdotV,F0);
            	float NDF = DistributionGGX(WN, WH, Roughness);       
				float G   = GeometrySmith(WN, WV,WL, Roughness);    
				float3 numerator = NDF * G * F;
				float denominator = 4.0 * NdotV * NdotL;
				float3 specular = numerator / max(denominator, 0.001);

				float3 kS = F;
				float3 kD = float3(1.0,1.0,1.0) - kS;
				kD *= 1.0 - Metallic;	

				UNITY_LIGHT_ATTENUATION(atten,i,WPOS);
				float3 PBRTerm = kD * Albedo  + specular;
				float3 finalColor = _LightColor0.rgb * PBRTerm * NdotL * atten;

				return finalColor;
            }	

            fixed4 MainLightFrag (v2f i) : SV_Target
            {

            	float3 finalColor = PBRLighting(i);
            	return fixed4(finalColor,1);
            }

            fixed4 LocalLightFrag (v2f i) : SV_Target
            {
            	float3 finalColor = PBRLighting(i);
                return fixed4(finalColor,1);
            }



        ENDCG

        Pass
        {
        	Tags { "LightMode"="ForwardBase" }
            CGPROGRAM
            #pragma multi_compile_fwdbase
            #pragma vertex vert
            #pragma fragment MainLightFrag


           
            ENDCG
        }

          Pass
        {
        	Tags { "LightMode"="ForwardAdd" }
			Blend One One

            CGPROGRAM
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fwdadd_fullshadows
            #pragma vertex vert
            #pragma fragment LocalLightFrag


           
            ENDCG
        }


    }

	FallBack "Specular"
}
