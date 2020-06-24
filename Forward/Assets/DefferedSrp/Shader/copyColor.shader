Shader "Hidden/copyColor"
{
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature __ _DEBUG_Normal _DEBUG_DEPTH _DEBUG_DEPTH01 _DEBUG_DEPTH_EYE

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            } 


            sampler2D _GAlbedo;
            sampler2D _GNormal;
            sampler2D _GDepth;

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = col = tex2D(_GAlbedo, i.uv);
           
#if _DEBUG_Normal  
                    col = tex2D(_GNormal, i.uv); 
#endif

#if _DEBUG_DEPTH
                    col = tex2D(_GDepth, i.uv);
#endif

#if _DEBUG_DEPTH01
                    col = tex2D(_GDepth, i.uv);
                    float depth = Linear01Depth(col.r);
                    col = float4(depth, depth, depth, 1);
#endif

#if _DEBUG_DEPTH_EYE
                    col = tex2D(_GDepth, i.uv);
                    float depth = LinearEyeDepth(col.r);
                    col = float4(depth, depth, depth, 1);
#endif
                   
                return col;
            }
            ENDCG
        }
    }
}
