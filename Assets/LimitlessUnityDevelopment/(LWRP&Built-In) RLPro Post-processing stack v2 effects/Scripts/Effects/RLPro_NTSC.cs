using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_NTSC_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/NTSC", false)]
public sealed class RLPro_NTSC : PostProcessEffectSettings
{
	[Range(1,40),Tooltip("Brightness.")]
	public FloatParameter brightness = new FloatParameter { value = 39.1f};
	[Range(0.01f, 2f), Tooltip("Blur size.")]
	public FloatParameter blur = new FloatParameter { value = 0.83f };
	[Range(0, 10), Tooltip("Floating lines speed")]
	public FloatParameter lineSpeed = new FloatParameter { value =  0.01f};
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter();
	public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

}

public sealed class RLPRO_SRP_NTSC_Renderer : PostProcessEffectRenderer<RLPro_NTSC>
{
	private float T;
	static readonly int _Mask = Shader.PropertyToID("_Mask");
	static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");

	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/NTSC_RLPro"));
		T += Time.deltaTime;
		sheet.properties.SetFloat("T", T);
		sheet.properties.SetFloat("Bsize", 41- settings.brightness.value);
		sheet.properties.SetFloat("val1", settings.lineSpeed.value);
		sheet.properties.SetFloat("val2", settings.blur.value);
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

		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
	private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
	{
		if (paramValue) mat.EnableKeyword(paramName);
		else mat.DisableKeyword(paramName);
	}

}
