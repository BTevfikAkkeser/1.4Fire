using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_Warp_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Warp", false)]
public sealed class RLProWarp : PostProcessEffectSettings
{
	[Range(0f, 1f), Tooltip("Dark areas adjustment.")]
	public FloatParameter fade = new FloatParameter { value = 1f };
	[Tooltip("Warp mode.")]
	public WarpModeParameter warpMode = new WarpModeParameter { };
	[Tooltip("Warp picture.")]
	public Vector2Parameter warp = new Vector2Parameter { value = new Vector2(0.03125f, 0.04166f) };
	public FloatParameter scale = new FloatParameter { value = 1f };
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter();
	public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

}

public sealed class RLPRO_SRP_Warp_Renderer : PostProcessEffectRenderer<RLProWarp>
{
	static readonly int _Mask = Shader.PropertyToID("_Mask");
	static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");

	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Warp_RLPro"));
		sheet.properties.SetFloat("fade", settings.fade);
		sheet.properties.SetFloat("scale", settings.scale);
		sheet.properties.SetVector("warp", settings.warp);
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

		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, settings.warpMode == WarpMode.SimpleWarp ? 0 : 1);
	}
	private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
	{
		if (paramValue) mat.EnableKeyword(paramName);
		else mat.DisableKeyword(paramName);
	}

}
