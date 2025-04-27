using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_EdgeNoise_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Edge Noise Effect", false)]
public sealed class RLProEdgeNoise : PostProcessEffectSettings
{
	public BoolParameter left = new BoolParameter { value = true };
	public BoolParameter right = new BoolParameter { value = true };
	public BoolParameter top = new BoolParameter { value = true };
	public BoolParameter bottom = new BoolParameter { value = true };
	[Range(0.01f, 0.5f), Tooltip("Height of Noise.")]
	public FloatParameter height = new FloatParameter { value = 0.2f };
	[Range(0f, 3f), Tooltip("Noise intensity.")]
	public FloatParameter intencity = new FloatParameter { value = 1.5f };
	[Tooltip("Noise texture.")]
	public TextureParameter noiseTexture = new TextureParameter { value = null };
	[Tooltip("Noise tiling.")]
	public Vector2Parameter tile = new Vector2Parameter { value = new Vector2(1, 1) };
}

public sealed class RLPRO_SRP_EdgeNoise_Renderer : PostProcessEffectRenderer<RLProEdgeNoise>
{
	private float T;
	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/EdgeNoiseEffect"));
		sheet.properties.SetFloat("_OffsetNoiseX", UnityEngine.Random.Range(0f, 1.0f));
		float offsetNoise1 = sheet.properties.GetFloat("_OffsetNoiseY");
		sheet.properties.SetFloat("_OffsetNoiseY", offsetNoise1 + UnityEngine.Random.Range(-0.05f, 0.05f));
		sheet.properties.SetFloat("_NoiseBottomHeight", settings.height);
		ParamSwitch(sheet, settings.top.value, "top_ON");
		ParamSwitch(sheet, settings.bottom.value, "bottom_ON");
		ParamSwitch(sheet, settings.left.value, "left_ON");
		ParamSwitch(sheet, settings.right.value, "right_ON");
		sheet.properties.SetFloat("_NoiseBottomIntensity", settings.intencity);
		if (settings.noiseTexture.value != null)
			sheet.properties.SetTexture("_SecondaryTex", settings.noiseTexture);
		sheet.properties.SetFloat("tileX", settings.tile.value.x);
		sheet.properties.SetFloat("tileY", settings.tile.value.y);
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
	private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
	{
		if (paramValue) mat.EnableKeyword(paramName);
		else mat.DisableKeyword(paramName);
	}

}
