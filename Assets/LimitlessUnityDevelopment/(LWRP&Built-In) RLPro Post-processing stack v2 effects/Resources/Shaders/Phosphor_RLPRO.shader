Shader "RetroLookPro/Phosphor_RLPro"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	sampler2D _MainTex;
	sampler2D _Tex;
	float speed = 10.00;
	half amount = 5;
	half fade;
	float T;
	#pragma shader_feature ALPHA_CHANNEL
	sampler2D _Mask;
	float _FadeMultiplier;

	float fract(float x) {
		return  x - floor(x);
	}
	float2 fract(float2 x) {
		return  x - floor(x);
	}

	float random(float2 noise)
	{
		return fract(sin(dot(noise.xy, float2(0.0001, 98.233))) * 925895933.14159265359);
	}

	float random_color(float noise)
	{
		return frac(sin(noise));
	}

	float4 Frag0(VaryingsDefault i) : SV_Target
	{
        float2 uv = i.texcoord;
		float4 col = tex2D(_MainTex,uv );
		float4 result = col + tex2D(_Tex, uv);
		return lerp(col,result,fade);
	}

		half4 Frag(VaryingsDefault i) : SV_Target
	{
        float2 uvs = i.texcoord;
		float2 uv = fract(uvs / 12 * ((T.x * speed)));
		half4 color = float4(random(uv.xy), random(uv.xy), random(uv.xy), random(uv.xy));

		color.r *= random_color(sin(T.x * speed));
		color.g *= random_color(cos(T.x * speed));
		color.b *= random_color(tan(T.x * speed));

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
				#pragma fragment Frag0

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}