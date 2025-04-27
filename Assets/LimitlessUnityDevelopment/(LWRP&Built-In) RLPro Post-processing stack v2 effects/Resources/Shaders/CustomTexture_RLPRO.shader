Shader "RetroLookPro/CustomTexture"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	sampler2D _MainTex;
	sampler2D _CustomTex;
	half fade;
	half alpha;
	float4 Frag(VaryingsDefault i) : SV_Target
	{
        float2 uv = i.texcoord;
		float4 col = tex2D(_MainTex,uv);
		float4 col2 = tex2D(_CustomTex,uv);

		if (alpha < 1)
			col2.a = fade;
		return lerp(col, col2, fade);

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