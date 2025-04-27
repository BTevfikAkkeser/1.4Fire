using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLProCRTAperture_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/CRT Aperture", false)]
public sealed class RLProCRTAperture : PostProcessEffectSettings
{
    [Range(0, 5), Tooltip(".")]
    public FloatParameter GlowHalation = new FloatParameter { value = 4.27f };
    [Range(0, 2), Tooltip(".")]
    public FloatParameter GlowDifusion = new FloatParameter { value = 0.83f };
    [Range(0, 2), Tooltip(".")]
    public FloatParameter MaskColors = new FloatParameter { value = 0.57f };
    [Range(0, 1), Tooltip(".")]
    public FloatParameter MaskStrength = new FloatParameter { value = 0.318f };
    [Range(0, 5), Tooltip(".")]
    public FloatParameter GammaInput = new FloatParameter { value = 1.12f };
    [Range(0, 5), Tooltip(".")]
    public FloatParameter GammaOutput = new FloatParameter { value = 0.89f };
    [Range(0, 2.5f), Tooltip(".")]
    public FloatParameter Brightness = new FloatParameter { value = 0.85f };
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter();
    public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

}

public sealed class RLProCRTAperture_Renderer : PostProcessEffectRenderer<RLProCRTAperture>
{
    static readonly int _Mask = Shader.PropertyToID("_Mask");
    static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/CRTAperture_RLPRO"));

        sheet.properties.SetFloat("GLOW_HALATION", settings.GlowHalation.value);
        sheet.properties.SetFloat("GLOW_DIFFUSION", settings.GlowDifusion.value);
        sheet.properties.SetFloat("MASK_COLORS", settings.MaskColors.value);
        sheet.properties.SetFloat("MASK_STRENGTH", settings.MaskStrength.value);
        sheet.properties.SetFloat("GAMMA_INPUT", settings.GammaInput.value);
        sheet.properties.SetFloat("GAMMA_OUTPUT", settings.GammaOutput.value);
        sheet.properties.SetFloat("BRIGHTNESS", settings.Brightness.value);
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
