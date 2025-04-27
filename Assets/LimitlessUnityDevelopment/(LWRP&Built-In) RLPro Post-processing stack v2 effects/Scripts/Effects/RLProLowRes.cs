using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_LowRes_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/LowRes", false)]
public sealed class RLProLowRes : PostProcessEffectSettings
{
	[Range(1, 20), Tooltip("Dark areas adjustment.")]
	public IntParameter pixelSize = new IntParameter { value = 1 };
	[Space]
	[Tooltip("Mask texture")]
	public TextureParameter mask = new TextureParameter();
	public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

}

public sealed class RLPRO_SRP_LowRes_Renderer : PostProcessEffectRenderer<RLProLowRes>
{
	static readonly int _Mask = Shader.PropertyToID("_Mask");
	static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");

	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/LowRes_RLPro"));
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


		Vector2Int res = new Vector2Int(Screen.width / settings.pixelSize, Screen.height / settings.pixelSize);
		RenderTexture scaled = RenderTexture.GetTemporary(res.x,res.y);
		scaled.filterMode = FilterMode.Point;
		context.command.BlitFullscreenTriangle(context.source, scaled, sheet, 1);
		sheet.properties.SetTexture("_ScaledMainTex", scaled);
		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
		RenderTexture.ReleaseTemporary(scaled);
	}
	private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
	{
		if (paramValue) mat.EnableKeyword(paramName);
		else mat.DisableKeyword(paramName);
	}

}
