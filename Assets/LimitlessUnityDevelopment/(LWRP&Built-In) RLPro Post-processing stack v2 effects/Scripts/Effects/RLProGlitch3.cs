using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(Glitch3Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Glitch3", false)]
public sealed class RLProGlitch3 : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Speed")]
    public FloatParameter speed = new FloatParameter { value = 1f };
    [Range(0f, 5f), Tooltip("block size (higher value = smaller blocks).")]
    public FloatParameter density = new FloatParameter { value = 1f };
    [Range(0f, 5f), Tooltip("glitch offset.(color shift)")]
    public FloatParameter maxDisplace = new FloatParameter { value = 1f };
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter();
    public maskChannelModeParameter maskChannel = new maskChannelModeParameter();
}

public sealed class Glitch3Renderer : PostProcessEffectRenderer<RLProGlitch3>
{
	private float T;
    static readonly int _Mask = Shader.PropertyToID("_Mask");
    static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");

    public override void Render(PostProcessRenderContext context)
    {
		T += Time.deltaTime;
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Glitch3"));
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
        sheet.properties.SetFloat("speed", settings.speed);
        sheet.properties.SetFloat("density", settings.density);
        sheet.properties.SetFloat("maxDisplace", settings.maxDisplace);
		sheet.properties.SetFloat("_TimeX", T);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }

}