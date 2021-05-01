Shader "Unlit/AtmoShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PlanetRadius("_PlanetRadius",Float) = 6371000
        _AtmosphereHeight("_AtmosphereHeight",Float) = 8000


    }
    SubShader
    {
       Tags{ "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
        LOD 100
        Cull Off
        Pass
        {
            Tags { "LightMode" = "ForwardBase"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase 
            #pragma target 5.0
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
                float3 vertex : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;


            #define PI 3.14159265359
            float _AtmosphereHeight;
            float _PlanetRadius;
            float2 _DensityScaleHeight;

            float3 _ScatteringR;
            float3 _ScatteringM;
            float3 _ExtinctionR;
            float3 _ExtinctionM;

            float4 _IncomingLight;
            float _MieG;

            float4 _FrustumCorners[4];

            float _SunIntensity;

            sampler2D _ParticleDensityLUT;
            sampler2D _RandomVectors;

            sampler3D _SkyboxLUT;
            sampler3D _SkyboxLUT2;

            sampler3D _InscatteringLUT;
            sampler3D _ExtinctionLUT;
            
            int _ScaterringSampleCount,_OpticalSampleCount;


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


float Sun(float cosAngle)
{
    float g = 0.98;
    float g2 = g * g;

    float sun = pow(1 - g, 2.0) / (4 * PI * pow(1.0 + g2 - 2.0*g*cosAngle, 1.5));
    return sun * 0.003;// 5;
}


void ApplyPhaseFunction(inout float3 scatterR, inout float3 scatterM, float cosAngle)
{
    // r
    float phase = (3.0 / (16.0 * PI)) * (1 + (cosAngle * cosAngle));
    scatterR *= phase;

    // m
    float g = _MieG;
    float g2 = g * g;
    phase = ((3.0) / (8.0 * PI)) * ( (1-g2)*(1 + cosAngle*cosAngle)) / ( (2 + g2) * pow((1 + g2 - 2*g * cosAngle),1.5)); 
    scatterM *= phase;
}

float3 RenderSun(in float3 scatterM, float cosAngle)
{
    return scatterM * Sun(cosAngle);
}


//CP
    bool CalLightSampling(float3 position,float3 L,out float2 opticalDepthCP)
    {
        opticalDepthCP = 0;
        float3 rayStart = position;
        float3 rayDir = -L;
        float3 planetCenter = float3(0, -_PlanetRadius, 0);
        float2 intersection = RaySphereIntersection(rayStart, rayDir, planetCenter, _PlanetRadius + _AtmosphereHeight);
        float3 rayEnd = rayStart + rayDir * intersection.y;

        float setpCount = _OpticalSampleCount;
        float3 step = (rayEnd - rayStart)/setpCount;
        float stepSize = length(step);
        float2 density = 0;

        for(float s = 0.5; s < setpCount; s += 1.0)
        {
            float3 position = rayStart +step * s;
            float height = abs(length(position - planetCenter) - _PlanetRadius);
            float2 localDensity = exp(-(height/_DensityScaleHeight));
            density += localDensity * stepSize;
        }
        opticalDepthCP =density;
        return true;

    }


//-----------------------------------------------------------------------------------------
// GetAtmosphereDensity
//-----------------------------------------------------------------------------------------
void GetAtmosphereDensity(float3 position, float3 planetCenter, float3 lightDir, out float2 localDensity, out float2 densityToAtmTop)
{
    float height = length(position - planetCenter) - _PlanetRadius;
    localDensity = exp(-height.xx / _DensityScaleHeight.xy);

    float cosAngle = dot(normalize(position - planetCenter), -lightDir.xyz);

    // densityToAtmTop = tex2D(_ParticleDensityLUT, float2(cosAngle * 0.5 + 0.5, (height / _AtmosphereHeight))).rg;
    CalLightSampling(position,lightDir,densityToAtmTop);
}

//-----------------------------------------------------------------------------------------
// ComputeLocalInscattering
//-----------------------------------------------------------------------------------------
void ComputeLocalInscattering(float2 localDensity, float2 densityPA, float2 densityCP, out float3 localInscatterR, out float3 localInscatterM)
{
    float2 densityCPA = densityCP + densityPA;

    float3 Tr = densityCPA.x * _ExtinctionR;
    float3 Tm = densityCPA.y * _ExtinctionM;

    float3 extinction = exp(-(Tr + Tm));

    localInscatterR = localDensity.x * extinction;
    localInscatterM = localDensity.y * extinction;
}

//-----------------------------------------------------------------------------------------
// ComputeOpticalDepth
//-----------------------------------------------------------------------------------------
float3 ComputeOpticalDepth(float2 density)
{
    float3 Tr = density.x * _ExtinctionR;
    float3 Tm = density.y * _ExtinctionM;

    float3 extinction = exp(-(Tr + Tm));

    return _IncomingLight.xyz * extinction;
}

//-----------------------------------------------------------------------------------------
// IntegrateInscattering
//-----------------------------------------------------------------------------------------
float4 IntegrateInscattering(float3 rayStart, float3 rayDir, float rayLength, float3 planetCenter, float distanceScale, float3 lightDir, float sampleCount, out float4 extinction)
{
    float3 step = rayDir * (rayLength / sampleCount);
    float stepSize = length(step) * distanceScale;

    float2 densityCP = 0;
    float3 scatterR = 0;
    float3 scatterM = 0;

    float2 localDensity;
    float2 densityPA;

    float2 prevLocalDensity;
    float3 prevLocalInscatterR, prevLocalInscatterM;
    GetAtmosphereDensity(rayStart, planetCenter, lightDir, prevLocalDensity, densityPA);
    ComputeLocalInscattering(prevLocalDensity, densityPA, densityCP, prevLocalInscatterR, prevLocalInscatterM);

    // P - current integration point
    // C - camera position
    // A - top of the atmosphere
    [loop]
    for (float s = 1.0; s < sampleCount; s += 1)
    {
        float3 p = rayStart + step * s;

        GetAtmosphereDensity(p, planetCenter, lightDir, localDensity, densityPA);
        densityCP += (localDensity + prevLocalDensity) * (stepSize / 2.0);

        prevLocalDensity = localDensity;

        float3 localInscatterR, localInscatterM;
        ComputeLocalInscattering(localDensity, densityPA, densityCP, localInscatterR, localInscatterM);
        
        scatterR += (localInscatterR + prevLocalInscatterR) * (stepSize / 2.0);
        scatterM += (localInscatterM + prevLocalInscatterM) * (stepSize / 2.0);

        prevLocalInscatterR = localInscatterR;
        prevLocalInscatterM = localInscatterM;
    }

    float3 m = scatterM;
    // phase function
    ApplyPhaseFunction(scatterR, scatterM, dot(rayDir, -lightDir.xyz));
    //scatterR = 0;
    float3 lightInscatter = (scatterR * _ScatteringR + scatterM * _ScatteringM) * _IncomingLight.xyz;
    lightInscatter += RenderSun(m, dot(rayDir, -lightDir.xyz)) * _SunIntensity;
    float3 lightExtinction = exp(-(densityCP.x * _ExtinctionR + densityCP.y * _ExtinctionM));

    extinction = float4(lightExtinction, 0);
    return float4(lightInscatter, 1);
}



            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                return o;
            }




    fixed4 frag (v2f i) : SV_Target
    {

        float3 rayStart = _WorldSpaceCameraPos;
        float3 rayDir = normalize(mul((float3x3)unity_ObjectToWorld, i.vertex));

        float3 lightDir = -_WorldSpaceLightPos0.xyz;

        float3 planetCenter = _WorldSpaceCameraPos;
        planetCenter = float3(0, -_PlanetRadius, 0);

        float2 intersection = RaySphereIntersection(rayStart, rayDir, planetCenter, _PlanetRadius + _AtmosphereHeight);     
        float rayLength = intersection.y;

        intersection = RaySphereIntersection(rayStart, rayDir, planetCenter, _PlanetRadius);
        if (intersection.x > 0)
            rayLength = min(rayLength, intersection.x);

        float4 extinction;
        float4 inscattering = IntegrateInscattering(rayStart, rayDir, rayLength, planetCenter, 1, lightDir, _ScaterringSampleCount, extinction);
        return float4(inscattering.xyz, 1);
    }
    ENDCG
        }
    }
}
