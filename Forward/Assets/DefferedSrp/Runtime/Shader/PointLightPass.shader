﻿Shader "Hidden/PointLightPass"
{
    SubShader
    {
      /*  Pass
        {
            Stencil
            {
                Ref 1
                Comp always
                ZFail Replace
            }

            Cull Front
            ZTest GEqual
            Zwrite Off
            Blend Zero One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
             struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) :SV_Target
            {
                return 0;
            }
       
            ENDCG

        }*/

        Pass
        {
            Cull Off
            Blend One One
            Ztest Always        
            Zwrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float3 ray : TEXCOORD1;
            };

            sampler2D _GAlbedo;
            sampler2D _GNormal;
            sampler2D _GDepth;
            sampler2D _DefferedDepth;

            float4 _LightDireciton;
            half4 _LightColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.ray = UnityObjectToViewPos(v.vertex) * float3(-1, -1, 1);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float4 albedo = tex2D(_GAlbedo, uv);
                float4 normal = tex2D(_GNormal, uv);

               
                float depth = SAMPLE_DEPTH_TEXTURE(_DefferedDepth, uv);
                depth = Linear01Depth(depth);
                float4 vpos = float4(i.ray * depth, 1);
                float3 wpos = mul(unity_CameraToWorld, vpos).xyz;

                float3 lightDir = _LightDireciton.xyz - wpos;
                float atten =  saturate(abs(_LightDireciton.w - length(lightDir)) / _LightDireciton.w);

                normal = normalize(normal);
                lightDir = normalize(lightDir);
                float diffuse = max(0, dot(lightDir, normal.xyz));
                
                return  albedo * diffuse * _LightColor *atten;
            }

            ENDCG
        }
    }
}
