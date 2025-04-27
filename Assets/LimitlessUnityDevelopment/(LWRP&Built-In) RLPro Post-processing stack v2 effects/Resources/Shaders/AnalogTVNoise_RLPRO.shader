Shader "RetroLookPro/AnalogTVNoise"
{
	HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	sampler2D _Pattern;
	sampler2D _MainTex;

	float _TimeX;
	half _Fade;
	half barHeight = 6.;
	half barOffset = 0.6;
	half barSpeed = 2.6;
	half barOverflow = 1.2;
	half edgeCutOff;
	half cut;
	half _OffsetNoiseX;
	half _OffsetNoiseY;
	half4 _MainTex_ST;
	half tileX = 0;
	half tileY = 0;
	half angle;
#pragma shader_feature ALPHA_CHANNEL
	sampler2D _Mask;
	float _FadeMultiplier;

	VaryingsDefault VertDef(AttributesDefault v)
	{
		VaryingsDefault o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
		o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
		o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
		float2 pivot = float2(0.5, 0.5);
		// Rotation Matrix
		float cosAngle = cos(angle);
		float sinAngle = sin(angle);
		float2x2 rot = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);
		// Rotation consedering pivot
		float2 uv = v.vertex.xy;
		float2 sfsf = mul(rot, uv);
		o.texcoordStereo = TransformStereoScreenSpaceTex(sfsf + o.texcoord + float2(_OffsetNoiseX - 0.2f, _OffsetNoiseY), 1.0);
		o.texcoordStereo *= float2(tileY, tileX);
		return o;
	}

	VaryingsDefault VertDefVertical(AttributesDefault v)
	{
		VaryingsDefault o;
		o.vertex = float4(v.vertex.xy, 0.0, 1.0);
		o.texcoord = TransformTriangleVertexToUV(v.vertex.xy);
		o.texcoord = o.texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
		float2 pivot = float2(1, 1);
		// Rotation Matrix
		float cosAngle = cos(angle);
		float sinAngle = sin(angle);
		float2x2 rot = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);
		// Rotation consedering pivot

		float2 uv = v.vertex.xy;
		float2 sfsf = mul(rot, uv);

		o.texcoordStereo = TransformStereoScreenSpaceTex(sfsf + o.texcoord + float2(_OffsetNoiseX, _OffsetNoiseY), 1.0);
		o.texcoordStereo *= float2(tileX, tileY);
		return o;
	}

	float4 Frag0(VaryingsDefault i) : SV_Target
	{
float2 uv = i.texcoord;
		float3 col;
		float4 text = tex2D(_MainTex, i.texcoord);
		float4 pat = tex2D(_Pattern, i.texcoordStereo);
		col.rgb = text.xyz;
		float bar = floor(edgeCutOff + sin(uv.y * barHeight + _TimeX * barSpeed) * 50);
		float f = clamp(bar * 0.03,0,1);
		if (_FadeMultiplier > 0)
		{
#if ALPHA_CHANNEL
			float alpha_Mask = step(0.0001, tex2D(_Mask, i.texcoord).a);
#else
			float alpha_Mask = step(0.0001, tex2D(_Mask, i.texcoord).r);
#endif
			_Fade *= alpha_Mask;
		}

		col = lerp(pat.rgb,col, f);
		col = lerp(text.rgb, col, smoothstep(col.r - cut,0,1) * _Fade);
		return float4(col,text.a);
	}

		float4 Frag(VaryingsDefault i) : SV_Target
	{
float2 uv = i.texcoord;
		float3 col;
		float4 text = tex2D(_MainTex, i.texcoord);
		float4 pat = tex2D(_Pattern, i.texcoordStereo);
		col.rgb = text.xyz;
		float bar = floor(edgeCutOff + sin(uv.x * barHeight + _TimeX * barSpeed) * 50);
		float f = clamp(bar * 0.03,0,1);
		if (_FadeMultiplier > 0)
		{
#if ALPHA_CHANNEL
			float alpha_Mask = step(0.0001, tex2D(_Mask, i.texcoord).a);
#else
			float alpha_Mask = step(0.0001, tex2D(_Mask, i.texcoord).r);
#endif
			_Fade *= alpha_Mask;
		}
		col = lerp(pat.rgb,col, f);
		col = lerp(text.rgb, col, smoothstep(col.r - cut,0,1) * _Fade);
		return float4(col,text.a);
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always
			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDef
				#pragma fragment Frag0

			ENDHLSL
		}
			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefVertical
				#pragma fragment Frag

			ENDHLSL
		}
	}
}