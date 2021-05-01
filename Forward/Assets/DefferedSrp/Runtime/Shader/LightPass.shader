Shader "Hidden/LightPass"
{

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend One Zero
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            sampler2D _GAlbedo;
            sampler2D _GNormal;
            sampler2D _GDepth;

            float4 _LightDireciton;
            half4 _LightColor;

            fixed4 frag(v2f i) : SV_Target
            {
                float4 albedo = tex2D(_GAlbedo,i.uv);
                float4 normal = tex2D(_GNormal,i.uv);
                float diffuse = max(0,dot(_LightDireciton.xyz, normal.xyz)) ;
                float depth = tex2D(_GDepth, i.uv);

                return diffuse * _LightColor * albedo ;
            }
            ENDCG
        } 
    }
}
