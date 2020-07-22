Shader "Hidden/PointLightPass"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        /*Pass
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

        }
        */




        Pass
        {
            /*Blend One Zero
            Cull Back
            Ztest LEqual
            Zwrite Off
            Stencil
            {
                Ref 1
                Comp EQUAL
                ZFail Keep
            }*/

            CGPROGRAM
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
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _GAlbedo;
            sampler2D _GNormal;
            sampler2D _GDepth;

            float4 _LightDireciton;
            half4 _LightColor;

           

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float4 albedo = tex2D(_GAlbedo, uv);
                float4 normal = tex2D(_GNormal, uv);
                float diffuse = max(0,dot(_LightDireciton.xyz, normal.xyz));
                float depth = tex2D(_GDepth, uv);

                return  albedo;
            }

            ENDCG
        }
    }
}
