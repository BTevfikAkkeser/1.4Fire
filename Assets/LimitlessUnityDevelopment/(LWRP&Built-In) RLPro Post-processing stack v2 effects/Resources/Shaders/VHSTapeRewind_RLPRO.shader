Shader "RetroLookPro/VHS_Tape_Rewind"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	sampler2D _MainTex;
	half fade;
	half intencity;	

	float4 Frag0(VaryingsDefault i) : SV_Target
	{
        float2 uv = i.texcoord;
		
		float2 displacementSampleUV = float2(uv.x + (_Time.y + 20) * 70, uv.y);

		float da = intencity;

		float displacement = tex2D(_MainTex, displacementSampleUV).x * da;

		float2 displacementDirection = float2(cos(displacement * 6.28318530718), sin(displacement * 6.28318530718));
		float2 displacedUV = (uv + displacementDirection * displacement);
		float4 shade = tex2D(_MainTex,  displacedUV);
		float4 main = tex2D(_MainTex,  uv);
		return float4(lerp(main, shade, fade));
	}

		

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always
			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag0

			ENDHLSL
		}

	}
}