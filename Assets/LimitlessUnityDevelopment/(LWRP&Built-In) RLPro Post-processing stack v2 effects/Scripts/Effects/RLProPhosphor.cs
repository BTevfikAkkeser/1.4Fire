using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_Phosphor_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Phosphor", false)]
public sealed class RLProPhosphor : PostProcessEffectSettings
{
	[Range(0f, 1f), Tooltip("fade.")]
	public FloatParameter fade = new FloatParameter { value = 1f };
	[Range(0f, 20f), Tooltip("width.")]
	public FloatParameter width = new FloatParameter { value = 0.4f };
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter();
	public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

}

public sealed class RLPRO_SRP_Phosphor_Renderer : PostProcessEffectRenderer<RLProPhosphor>
{
	private RenderTexture texTape = null;
	bool stop;
	static readonly int _Mask = Shader.PropertyToID("_Mask");
	static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");


	float T;
	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Phosphor_RLPro"));
		if (texTape == null)
		{
			texTape = new RenderTexture(Screen.width, Screen.height, 1);

		}
		context.command.BlitFullscreenTriangle(context.source, texTape, sheet, 1);
		sheet.properties.SetTexture("_Tex", texTape);
		T = Time.time;
		sheet.properties.SetFloat("T", T);
		sheet.properties.SetFloat("speed", settings.width.value);
		sheet.properties.SetFloat("fade", settings.fade.value);
		if (settings.mask.value != null)
		{
			sheet.properties.SetTexture(_Mask, settings.mask.value);
			sheet.properties.SetFloat(_FadeMultiplier, 1);
			ParamSwitch(sheet, settings.maskChannel.value == maskChannelMode.alphaChannel ? true : false, "ALPHA_CHANNEL");
		}
		else
		{
			sheet.properties.SetFloat(_FadeMultiplier, 0);
		}

		texTape.Release();
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
	private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
	{
		if (paramValue) mat.EnableKeyword(paramName);
		else mat.DisableKeyword(paramName);
	}

}
