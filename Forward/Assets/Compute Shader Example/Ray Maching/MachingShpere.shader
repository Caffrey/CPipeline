Shader "Custom/MachingShpere"
{
    
    SubShader
    {
        Tags { "Queue"="Transparent" }
        

        Pass
        {

            Blend SrcAlpha OneMinusSrcAlpha

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
                float3 wPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wPos =  mul(unity_ObjectToWorld,v.vertex).xyz;
                return o;
            }

            #define STEPS 64
            #define STEP_SIZE 0.01

            bool SphereHit(float3 p ,float3 center, float radius)
            {
                return distance(p,center) < radius;
            }

            float RaymachHit(float3 position, float3 direction)
            {
                for(int i = 0; i < STEPS; i++)
                {
                    if(SphereHit(position,float3(0,0,0),0.5))
                    {
                        return position;
                    }
                    position += direction * STEP_SIZE;
                }

                return 0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 V = normalize(i.wPos - _WorldSpaceCameraPos);
                float3 Wpos = i.wPos;
                float depth = RaymachHit(Wpos,V);

                 if(depth != 0)
                 {
                    return fixed4(1,0,0,1);
                 }
                 else
                 {
                    return fixed4(0,0,0,0);
                 }

            }
            ENDCG
        }
    }
}
