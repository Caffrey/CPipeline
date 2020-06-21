Shader "Unlit/RayMarchingSphere"
{
    Properties
    {
        _Color("Color",Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _Center("Center", Vector) = (0,0,0,0)
        _Radius("Radius",Range(0,5)) = 0.5 
        _MinDistance("MinDistance",Range(0.0001,1)) = 0
        _SpecularPower("SpecularPower",Float) =1
        _Gloss("Gloss",Float) =1
    }

    CGINCLUDE 

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 WPOS : TEXCOORD1;

            };


#define STEPS 256
#define MIN_DISTANCE 0 　

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Center;
            float _Radius,_MinDistance,_SpecularPower,_Gloss;
            fixed4 _Color;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.WPOS = mul(unity_ObjectToWorld,v.vertex).xyz;
                return o;
            }


            float map(float3 p)
            {
                return distance(p,_Center)- _Radius;
            }


            float3 normal(float3 p)
            {
                const float eps = 0.01;
                return normalize(
                    float3(
                        map(p + float3(eps,0,0)) - map(p-float3(eps,0,0)),
                        map(p + float3(0,eps,0)) - map(p-float3(0,eps,0)),
                        map(p + float3(0,0,eps)) - map(p-float3(0,0,eps))
                ));
            }

            fixed4 simpleLambert (fixed3 normal,fixed3 V) {
                fixed3 lightDir = _WorldSpaceLightPos0.xyz; // Light direction
                fixed3 lightCol = _LightColor0.rgb;     // Light color

                fixed NdotL = max(dot(normal, lightDir),0);
                fixed4 c = fixed4(0,0,0,0);

                fixed3 h = (lightDir - V) / 2.;
                fixed s = pow( dot(normal, h), _SpecularPower) * _Gloss;
                c.rgb += _Color * lightCol * NdotL + s;
                c.a = 1;
                return c;
            }

          

            fixed4 raymarchHit(float3 P, float3 D)
            {
                for(int i = 0; i < STEPS; i++)
                {
                    float distance = map(P);

                    if(distance < _MinDistance)
                    {
                        return simpleLambert(normal(P),D);
                    }
                    P += distance * D;
                }
                return fixed4(1,1,1,1);
            }
            
            

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 V = normalize(i.WPOS - _WorldSpaceCameraPos);
                return raymarchHit(i.WPOS,V);
            }

    ENDCG


    SubShader
    {
       
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            ENDCG
        } 
    }
}
