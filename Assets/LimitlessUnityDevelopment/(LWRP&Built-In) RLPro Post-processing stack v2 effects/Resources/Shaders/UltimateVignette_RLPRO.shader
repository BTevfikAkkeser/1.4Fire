Shader "RetroLookPro/UltimateVignette_RLPro"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
    #pragma fragmentoption ARB_precision_hint_fastest 
	sampler2D _MainTex;
	float4 _MainTex_ST;
	half4 _Params;
	half3 _InnerColor;
	half4 _Center;
    #pragma shader_feature VIGNETTE_CIRCLE
    #pragma shader_feature VIGNETTE_SQUARE
    #pragma shader_feature VIGNETTE_ROUNDEDCORNERS
	half2 _Params1;

	float4 Frag(VaryingsDefault i) : SV_Target
	{
        float2 uv = i.texcoord;

		half4 color = tex2D(_MainTex, uv);

		#if VIGNETTE_CIRCLE
		half d = distance(uv, _Center.xy);
		half multiplier = smoothstep(0.8, _Params.x * 0.799, d * (_Params.y + _Params.x));
		#elif VIGNETTE_ROUNDEDCORNERS
		half2 uv2 = -uv * uv + uv;
		half v = saturate(uv2.x * uv2.y * _Params1.x + _Params1.y);
		half multiplier = smoothstep(0.8, _Params.x * 0.799, v * (_Params.y + _Params.x));
		#else
		half multiplier = 1.0;
		#endif
		_InnerColor = -_InnerColor;
		color.rgb = (color.rgb - _InnerColor) * max((1.0 - _Params.z * (multiplier - 1.0) - _Params.w), 1.0) + _InnerColor;
		color.rgb *= multiplier;

		return color;
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