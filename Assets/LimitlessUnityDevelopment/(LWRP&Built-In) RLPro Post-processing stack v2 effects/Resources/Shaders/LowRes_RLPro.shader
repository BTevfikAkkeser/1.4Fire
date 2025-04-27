Shader "RetroLookPro/LowRes_RLPro"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	sampler2D _MainTex;
	sampler2D _ScaledMainTex;
#pragma shader_feature ALPHA_CHANNEL
	sampler2D _Mask;
	float _FadeMultiplier;

		float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 pos = i.texcoord;
		float texelHeightOffset = 0.001;
		float texelWidthOffset = 0.001;
		float2 firstOffset = float2(texelWidthOffset, texelHeightOffset);
		float2 secondOffset = float2(1.0 * texelWidthOffset, 1.0 * texelHeightOffset);
		float2 thirdOffset = float2(2.0 * texelWidthOffset, 2.0 * texelHeightOffset);
		float2 fourthOffset = float2(3.0 * texelWidthOffset, 3.0 * texelHeightOffset);

		float2 centerTextureCoordinate = pos;
		float2 oneStepLeftTextureCoordinate = pos - firstOffset;
		float2 twoStepsLeftTextureCoordinate = pos - secondOffset;
		float2 threeStepsLeftTextureCoordinate = pos - thirdOffset;
		float2 fourStepsLeftTextureCoordinate = pos - fourthOffset;
		float2 oneStepRightTextureCoordinate = pos + firstOffset;
		float2 twoStepsRightTextureCoordinate = pos + secondOffset;
		float2 threeStepsRightTextureCoordinate = pos + thirdOffset;
		float2 fourStepsRightTextureCoordinate = pos + fourthOffset;

		float4 fragmentColor = tex2D(_ScaledMainTex, centerTextureCoordinate) * 0.38026;

		fragmentColor += tex2D(_ScaledMainTex, oneStepLeftTextureCoordinate) * 0.27667;
		fragmentColor += tex2D(_ScaledMainTex, oneStepRightTextureCoordinate) * 0.27667;

		fragmentColor += tex2D(_ScaledMainTex, twoStepsLeftTextureCoordinate) * 0.08074;
		fragmentColor += tex2D(_ScaledMainTex, twoStepsRightTextureCoordinate) * 0.08074;

		fragmentColor += tex2D(_ScaledMainTex, threeStepsLeftTextureCoordinate) * -0.02612;
		fragmentColor += tex2D(_ScaledMainTex, threeStepsRightTextureCoordinate) * -0.02612;

		fragmentColor += tex2D(_ScaledMainTex, fourStepsLeftTextureCoordinate) * -0.02143;
		fragmentColor += tex2D(_ScaledMainTex, fourStepsRightTextureCoordinate) * -0.02143;

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

		return lerp(col1, fragmentColor, fade);

	}
	float4 Frag1(VaryingsDefault i) : SV_Target
	{
        float2 pos = i.texcoord;
		float4 col = tex2D(_MainTex,pos);

		return col;
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
			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag1

			ENDHLSL
		}
	}
}