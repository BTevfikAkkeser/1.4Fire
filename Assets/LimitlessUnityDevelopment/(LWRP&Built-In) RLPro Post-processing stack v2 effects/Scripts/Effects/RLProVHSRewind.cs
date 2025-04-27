using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_VHSRewind), PostProcessEvent.BeforeStack, "Retro Look Pro/VHS Tape Rewind", false)]
public sealed class RLProVHSRewind : PostProcessEffectSettings
{
	[Range(0f, 1f), Tooltip("Fade adjustment.")]
	public FloatParameter fade = new FloatParameter { value = 1f };
	[Range(0f, 5f), Tooltip("Intencity adjustment.")]
	public FloatParameter intencity = new FloatParameter { value = 0.57f };
}

public sealed class RLPRO_SRP_VHSRewind : PostProcessEffectRenderer<RLProVHSRewind>
{
	static readonly int intencity = Shader.PropertyToID("intencity");
	static readonly int fade = Shader.PropertyToID("fade");

	public override void Render(PostProcessRenderContext context)
	{
		var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/VHS_Tape_Rewind"));
		sheet.properties.SetFloat(fade, settings.fade);
		sheet.properties.SetFloat(intencity, settings.intencity);
		

		context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
	}
}
