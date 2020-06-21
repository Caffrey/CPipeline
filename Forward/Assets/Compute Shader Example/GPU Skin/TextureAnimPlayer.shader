Shader "Unlit/TextureAnimPlayer"
{
    Properties
    {
       _MainTex("Texture", 2D) = "white" {}
        _PosTex("position texture", 2D) = "black"{}
        _NmlTex("normal texture", 2D) = "white"{}
        _DT("delta time", float) = 0
        _Length("animation length", Float) = 1
        [Toggle(ANIM_LOOP)] _Loop("loop", Float) = 0

        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile __ ANIM_LOOP

            #include "UnityCG.cginc"

            struct appdata
            {
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1; 
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _PosTex, _NmlTex;
			float4 _PosTex_TexelSize;
			float _Length, _DT;
            #define ts _PosTex_TexelSize

            v2f vert (appdata v, uint vid : SV_VertexID)
            {
                float t = (_Time.y - _DT) / _Length;
#if ANIM_LOOP
                t = fmod(t, 1.0);
#else
                t = saturate(t);
#endif

                    float x = (vid + 0.5) * ts.x;
                    float y = t;
                    float4 pos = tex2Dlod(_PosTex, float4(x, y, 0, 0));
                    float3 normal = tex2Dlod(_NmlTex, float4(x, y, 0, 0));

                    v2f o;
                    o.vertex = UnityObjectToClipPos(pos);
                    o.normal = UnityObjectToWorldNormal(normal);
                    o.uv = v.uv;
                    return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                half diff = dot(i.normal,float3(0,1,0)) * 0.5 + 0.5;

                return diff * col;
            }
            ENDCG
        }
    }
}
