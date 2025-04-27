Shader "RetroLookPro/EdgeNoiseEffect"
{
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	half tileX = 0;
	half tileY = 0;
	half _OffsetNoiseX;
	half _OffsetNoiseY;
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _SecondaryTex;
	#pragma shader_feature top_ON
	#pragma shader_feature bottom_ON
	#pragma shader_feature left_ON
	#pragma shader_feature right_ON

	half _OffsetPosY;
	half _NoiseBottomHeight;
	half _NoiseBottomIntensity;
	VaryingsDefault VertDef(AttributesDefault v)
	{
		VaryingsDefault o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
		o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
		o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
		o.texcoordStereo = o.texcoord + float2(_OffsetNoiseX - 0.2f, _OffsetNoiseY);
		o.texcoordStereo *= float2(tileY, tileX);
		return o;
	}
	float4 Frag(VaryingsDefault i) : SV_Target
	{
 		half2 uv = i.texcoord;
		float4 outColor = tex2D(_MainTex, uv);
		float condition = 0;
		float4 noise_bottom = float4(0, 0, 0, 0);
		float val = 1;
		int count = 1;
		_NoiseBottomHeight -= 0.099;
#if top_ON
		condition = saturate(floor(uv.y / (1 - _NoiseBottomHeight)));
		noise_bottom = tex2D(_SecondaryTex, i.texcoordStereo) * condition * _NoiseBottomIntensity;
		val = (1 - _NoiseBottomHeight) / uv.y;
		outColor = lerp(outColor, noise_bottom, -noise_bottom * (val - 1.1));
#endif
#if bottom_ON
		condition = saturate(floor(_NoiseBottomHeight / uv.y));
		noise_bottom = tex2D(_SecondaryTex, i.texcoordStereo) * condition * _NoiseBottomIntensity;
		val = uv.y / (_NoiseBottomHeight);
		outColor += lerp(outColor, noise_bottom, -noise_bottom * (val - 1.0));
		count += 1;
		outColor /= 2;

#endif
#if left_ON
		condition = saturate(floor(_NoiseBottomHeight / uv.x));
		noise_bottom = tex2D(_SecondaryTex, i.texcoordStereo) * condition * _NoiseBottomIntensity;
		val = uv.x / _NoiseBottomHeight;
		outColor += lerp(outColor, noise_bottom, -noise_bottom * (val - 1.0));
		outColor /= 2;

#endif
#if right_ON
		condition = saturate(floor(uv.x / (1 - _NoiseBottomHeight))) * 3;
		noise_bottom = tex2D(_SecondaryTex, i.texcoordStereo) * condition * _NoiseBottomIntensity;
		val = (1 - _NoiseBottomHeight) / uv.x;
		outColor += lerp(outColor, noise_bottom, -noise_bottom * (val - 1.0));
		outColor /= 2;
#endif

		return float4(pow(outColor.xyz, float3(1.0, 1.0, 1.0)), outColor.a);
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDef
				#pragma fragment Frag

			ENDHLSL
		}
	}
}