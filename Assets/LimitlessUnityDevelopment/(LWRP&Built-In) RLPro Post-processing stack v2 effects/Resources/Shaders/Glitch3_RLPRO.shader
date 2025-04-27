Shader "RetroLookPro/Glitch3"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	sampler2D _MainTex;
	half alphaTex;
	sampler2D _AlphaMapTex;
	float _TimeX;
	float speed;
	float density;
	float maxDisplace;
#pragma shader_feature ALPHA_CHANNEL
	sampler2D _Mask;
	float _FadeMultiplier;

	inline float rand(float2 seed)
	{
		return frac(sin(dot(seed * floor(_TimeX * speed*20), float2(127.1, 311.7))) * 43758.5453123);
	}

	inline float rand(float seed)
	{
		return rand(float2(seed, 1.0));
	}

	float4 Frag(VaryingsDefault i) : SV_Target
	{
        float2 uv = i.texcoord;
		if (_FadeMultiplier > 0)
		{
#if ALPHA_CHANNEL
			float alpha_Mask = step(0.0001, tex2D(_Mask, uv).a);
#else
			float alpha_Mask = step(0.0001, tex2D(_Mask, uv).r);
#endif
			density *= alpha_Mask;
			maxDisplace *= alpha_Mask;
		}

		float2 rblock = rand(floor(uv * density));
		float displaceNoise = pow(rblock.x, 8.0) * pow(rblock.x, 3.0) - pow(rand(7.2341), 17.0) * maxDisplace;
		float cuttOff = 1;
		if (alphaTex > 0)
			cuttOff = tex2D(_AlphaMapTex, uv).a;
		float4 r = tex2D(_MainTex, uv);
		float4 g = tex2D(_MainTex, uv + half2(displaceNoise * 0.05 * rand(7.0), 0.0) * cuttOff);
		float4 b = tex2D(_MainTex, uv - half2(displaceNoise * 0.05 * rand(13.0), 0.0) * cuttOff);

		return half4(r.r, g.g, b.b, r.a + g.a + b.a);
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}