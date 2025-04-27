Shader "RetroLookPro/EdgeStretchEffect"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	sampler2D _MainTex;
	float amplitude;
	float frequency;
	half _NoiseBottomHeight;
	half speed;
	float _TimeX;
	#pragma shader_feature top_ON
	#pragma shader_feature bottom_ON
	#pragma shader_feature left_ON
	#pragma shader_feature right_ON
	float Time;
	float onOff(float a, float b, float c, float t)
	{
		return step(c, sin(t + a * cos(t * b)));
	}
	float2 wobble(float2 uv, float amplitude, float frequence, float speed)
	{
		float offset = amplitude * sin(uv.y * frequence * 20.0 + Time * speed);
		return float2((uv.x + (20 * _NoiseBottomHeight)) + offset, uv.y);
	}
	float2 wobbleR(float2 uv, float amplitude, float frequence, float speed)
	{
		float offset = amplitude * onOff(2.1, 4.0, 0.3, Time * speed) * sin(uv.y * frequence * 20.0 + Time * speed);
		return float2((uv.x + (20 * _NoiseBottomHeight)) + offset, uv.y);
	}
	float2 wobbleV(float2 uv, float amplitude, float frequence, float speed)
	{
		float offset = amplitude * sin(uv.x * frequence * 20.0 + Time * speed);
		return float2((uv.y + (20 * _NoiseBottomHeight)) + offset, uv.x);
	}
	float2 wobbleVR(float2 uv, float amplitude, float frequence, float speed)
	{
		float offset = amplitude * onOff(2.1, 4.0, 0.3, Time * speed) * sin(uv.x * frequence * 20.0 + Time * speed);
		return float2((uv.y + (20 * _NoiseBottomHeight)) + offset, uv.x);
	}

	float4 FragDist(VaryingsDefault i) : SV_Target
	{
		#if top_ON
			i.texcoord.y = min(i.texcoord.y, 1 - (wobble(i.texcoord, amplitude, frequency, speed).x * (_NoiseBottomHeight / 20)));

		#endif
		#if bottom_ON
			i.texcoord.y = max(i.texcoord.y, wobble(i.texcoord,amplitude, frequency, speed).x * (_NoiseBottomHeight / 20));
		#endif
		#if left_ON
			i.texcoord.x = max(i.texcoord.x, wobbleV(i.texcoord, amplitude, frequency, speed).x * (_NoiseBottomHeight / 20));
		#endif
		#if right_ON
			i.texcoord.x = min(i.texcoord.x, 1 - (wobbleV(i.texcoord, amplitude, frequency, speed).x * (_NoiseBottomHeight / 20)));
		#endif
		half2 positionSS = i.texcoord;
		half4 color = tex2D(_MainTex, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}
		float4 FragDistRand(VaryingsDefault i) : SV_Target
	{
		#if top_ON
			i.texcoord.y = min(i.texcoord.y, 1 - (wobbleR(i.texcoord, amplitude, frequency, speed).x * (_NoiseBottomHeight / 20)));
		#endif
		#if bottom_ON
			i.texcoord.y = max(i.texcoord.y, wobbleR(i.texcoord, amplitude, frequency, speed).x * (_NoiseBottomHeight / 20));
		#endif
		#if left_ON
			i.texcoord.x = max(i.texcoord.x, wobbleVR(i.texcoord, amplitude, frequency, speed).x * (_NoiseBottomHeight / 20));
		#endif
		#if right_ON
			i.texcoord.x = min(i.texcoord.x, 1 - (wobbleVR(i.texcoord, amplitude, frequency, speed).x * (_NoiseBottomHeight / 20)));
		#endif
		half2 positionSS = i.texcoord;
		half4 color = tex2D(_MainTex, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}
		float4 Frag(VaryingsDefault i) : SV_Target
	{
		 half2 positionSS = i.texcoord;
		#if top_ON
				 positionSS.y = min(positionSS.y, 1 - (_NoiseBottomHeight / 2) - 0.01);
		#endif
		#if bottom_ON
				 positionSS.y = max(positionSS.y, (_NoiseBottomHeight / 2) - 0.01);
		#endif
		#if left_ON
				 positionSS.x = max(positionSS.x, (_NoiseBottomHeight / 2) - 0.01);
		#endif
		#if right_ON
				 positionSS.x = min(positionSS.x, 1 - (_NoiseBottomHeight / 2) - 0.01);
		#endif

		half4 color = tex2D(_MainTex, positionSS);
		float exp = 1.0;
		return float4(pow(color.xyz, float3(exp, exp, exp)), color.a);
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment FragDist

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment FragDistRand

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