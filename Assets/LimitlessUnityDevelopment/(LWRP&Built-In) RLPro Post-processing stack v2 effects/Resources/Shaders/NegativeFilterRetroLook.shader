Shader "RetroLookPro/NegativeFilterRetroLook"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	uniform sampler2D _MainTex;
	uniform float Luminosity;
	uniform float Vignette;
	uniform float Negative;
#pragma shader_feature ALPHA_CHANNEL
	sampler2D _Mask;
	float _FadeMultiplier;

	float3 linearLight(float3 s, float3 d)
	{
		return 2.0 * s + d - 1.0 * Luminosity;
	}
	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 uv = i.texcoord;
		float3 col = tex2D(_MainTex, uv).rgb;
		col = lerp(col,1 - col,Negative);
		col *= pow(abs(16.0 * uv.x * (1.0 - uv.x) * uv.y * (1.0 - uv.y)), 0.4) * 1 + Vignette;
		col = dot(float3(0.2126, 0.7152, 0.0722), col);
		col = linearLight(float3(0.5,0.5,0.5),col);

		half4 col1 = tex2D(_MainTex, i.texcoord);

		float fade = 1;

		if (_FadeMultiplier > 0)
		{
#if ALPHA_CHANNEL
			float alpha_Mask = step(0.0001, tex2D(_Mask, i.texcoord).a);
#else
			float alpha_Mask = step(0.0001, tex2D(_Mask, i.texcoord).r);
#endif
			fade *= alpha_Mask;
		}

		return lerp(col1, float4(col, col1.a), fade);

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