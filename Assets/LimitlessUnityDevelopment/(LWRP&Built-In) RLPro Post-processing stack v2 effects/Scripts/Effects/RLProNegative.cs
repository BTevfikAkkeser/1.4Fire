using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_NegativeRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Negative Filter", false)]
public sealed class RLProNegative : PostProcessEffectSettings
{
    [Range(0f, 2f), Tooltip("Brightness.")]
    public FloatParameter luminosity = new FloatParameter { value = 1f };
    [Range(0f, 1f), Tooltip("Vignette amount.")]
    public FloatParameter vignette = new FloatParameter { value = 1f };

    [Range(0f, 1f), Tooltip("Negative amount.")]
    public FloatParameter negative = new FloatParameter { value = 0.88f };
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter();
    public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

}

public sealed class RLPRO_SRP_NegativeRenderer : PostProcessEffectRenderer<RLProNegative>
{
    private float T;
    static readonly int _Mask = Shader.PropertyToID("_Mask");
    static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");


    public override void Render(PostProcessRenderContext context)
    {
        T += Time.deltaTime;
        if (T > 100) T = 0;

        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/NegativeFilterRetroLook"));
        sheet.properties.SetFloat("T", T);
        sheet.properties.SetFloat("Luminosity", 2 - settings.luminosity);
        sheet.properties.SetFloat("Vignette", 1 - settings.vignette);
        sheet.properties.SetFloat("Negative", settings.negative);
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