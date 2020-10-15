Shader "Unlit/AtmoShader2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlanetRadius("_PlanetRadius",Float) = 1
        _AtmosphereHeight("_AtmosphereHeight",Float) = 1
        _ScatteringCoefficients("_ScatteringCoefficients",Float) = 1
        _DensityFallOff("_DensityFallOff",Float) = 4
        _NumInScaterringPoint("_NumInScaterringPoint",Float) = 10
        _NumOpticalDepthPoint("_NumOpticalDepthPoint",Float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        LOD 100

        Pass
        {

            Tags{"LightMode" = "ForwardBase"}


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _PlanetRadius, _AtmosphereHeight,  _ScatteringCoefficients, _DensityFallOff;
            int _NumInScaterringPoint, _NumOpticalDepthPoint;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                return o;
            }

             float2 RaySphereIntersection(float3 rayOrigin, float3 rayDir, float3 sphereCenter, float sphereRadius)
            {
                rayOrigin -= sphereCenter;
                float a = dot(rayDir, rayDir);
                float b = 2.0 * dot(rayOrigin, rayDir);
                float c = dot(rayOrigin, rayOrigin) - (sphereRadius * sphereRadius);
                float d = b * b - 4 * a * c;
                if (d < 0)
                {
                    return -1;
                }
                else
                {
                    d = sqrt(d);
                    return float2(-b - d, -b + d) / (2 * a);
                }
            }


            float densityAtPoint(float3 densitySamplePoint)
            {   
                float3 planeCenter = float3(0,-_PlanetRadius,0); 
                //海波高度
                float heightAboveSurface = length(densitySamplePoint - planeCenter) - _PlanetRadius;
                float height01 = heightAboveSurface / (_AtmosphereHeight - _PlanetRadius);
                float localDensity = exp(-height01 * _DensityFallOff) * (1 - height01);
                return localDensity;
            }

            float opticalDepth(float3 rayOrgin, float3 rayDir, float rayLength)
            {
                float3 DensitySamplePoint = rayOrgin;
                float stepSize = rayLength / (_NumOpticalDepthPoint - 1);
                float opticalDepth = 0;

                for(int i = 0; i < _NumOpticalDepthPoint; i++)
                {
                    float localDensity = densityAtPoint(DensitySamplePoint);
                    opticalDepth += localDensity * stepSize;
                    DensitySamplePoint += rayDir * stepSize;
                }


                return opticalDepth;
            }

            //rayOrigin = B点
            //RayDire：向量BA ， A点=相机位置
            //RayLength : 相机与观看点的物理距离
            float3 calculateLight(float3 rayOrigin, float3 rayDir, float rayLength)
            {
                float3 inScatterPoint = rayOrigin;
                float stepSize = rayLength / _NumInScaterringPoint;
                float3 inScatterLight = 0;

                float ViewRayOpticalDepth;
                float3 planeCenter = float3(0,-_PlanetRadius,0); 
                float3 RaySunDir = _WorldSpaceLightPos0.xyz;

                //Inscattering Light

                for(int i = 0; i < _NumInScaterringPoint; i++)
                {
                    //计算CP的InScattering
                    float SunRayLength = RaySphereIntersection(inScatterPoint, RaySunDir, planeCenter, _PlanetRadius + _AtmosphereHeight).y;
                    //CP的光学距离
                    float SunRayOpticalDepth = opticalDepth(inScatterLight, RaySunDir, SunRayLength);
                    //PA的光学距离, 这个循环是步进PA的,PA每一个点上都会有步进
                    ViewRayOpticalDepth += opticalDepth(rayOrigin, -rayDir, stepSize * i);
                    //T项衰减因子
                    float transmittance = exp(-(SunRayOpticalDepth + ViewRayOpticalDepth) * _ScatteringCoefficients);
                    //PH
                    float localDensity = densityAtPoint(inScatterPoint);

                    //Is * B(lunda) * r(theate) * 和T(CP) * T(PA) * p(h) * ds
                    //p(h)密度
                    //
                    inScatterLight += localDensity * transmittance * _ScatteringCoefficients * stepSize;
                    inScatterPoint += rayDir * stepSize;
                }





                return inScatterLight;
            }


            fixed4 frag (v2f i) : SV_Target
            {

                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(_WorldSpaceLightPos0);


                float3 planeCenter = float3(0,-_PlanetRadius,0); 
                //求B点
                float2 hitInfo = RaySphereIntersection(rayOrigin,rayDir, planeCenter, _PlanetRadius + _AtmosphereHeight);
                float3 pointIntersectAtmosphere = rayOrigin + rayDir * hitInfo.y;

                float3 light = calculateLight(pointIntersectAtmosphere,rayDir,hitInfo.y);

                return float4(light,1);


                // return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}
