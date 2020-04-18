#ifndef CUSTOM_SHADOW_CASTER_PASS_INCLUDED
#define CUSTOM_SHADOW_CASTER_PASS_INCLUDED

#include "ShaderLibrary/common.hlsl"

TEXTURE2D(_BaseMap);
SAMPLER(sampler_BaseMap);

 
struct Attributes {
	float3 positionOS : POSITION;
	float2 baseUV : TEXCOORD0;
};

struct Varyings {
	float4 positionCS : SV_POSITION;
	float2 baseUV : VAR_BASE_UV;
};

Varyings ShadowCasterPassVertex (Attributes input) {
	Varyings output;
	float3 positionWS = TransformObjectToWorld(input.positionOS);
	output.positionCS = TransformWorldToHClip(positionWS);

	output.baseUV = input.baseUV;
	return output;
}

void ShadowCasterPassFragment (Varyings input) {
}

#endif