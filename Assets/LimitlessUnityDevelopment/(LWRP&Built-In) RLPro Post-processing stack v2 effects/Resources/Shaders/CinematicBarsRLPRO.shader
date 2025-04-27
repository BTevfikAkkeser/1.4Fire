Shader "RetroLookPro/CinematicBars"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	sampler2D _MainTex;
	half _Stripes;
	half _Fade;

	float4 Frag(VaryingsDefault i) : SV_Target
	{
        float2 uv = i.texcoord;
		float4 col = tex2D(_MainTex, uv);
		float4 col2 = tex2D(_MainTex,uv);
		col *= (1 - ceil(saturate(abs(uv.y - 0.5) - _Stripes)));
		return lerp(col2,col,_Fade);
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