using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(Glitch1Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Glitch1", false)]
public sealed class RLProGlitch1 : PostProcessEffectSettings
{
    [Range(0f, 2f), Tooltip("Effect amount.")]
    public FloatParameter amount = new FloatParameter { value = 1f };
    [Space]
    [Range(0f, 4f), Tooltip("Effect stretch.")]
    public FloatParameter stretch = new FloatParameter { value = 0.02f };
    [Range(0f, 1f), Tooltip("Effect speed.")]
    public FloatParameter speed = new FloatParameter { value = 0.5f };
    [Range(0f, 1f), Tooltip("Effect Fade.")]
    public FloatParameter fade = new FloatParameter { value = 0.5f };
    [Space]
    [Range(-1f, 2f), Tooltip("Red color offset  muliplier.")]
    public FloatParameter rMultiplier = new FloatParameter { value = 1f };
    [Range(-1f, 2f), Tooltip("Green color offset  muliplier.")]
    public FloatParameter gMultiplier = new FloatParameter { value = 1f };
    [Range(-1f, 2f), Tooltip("Blue color offset  muliplier.")]
    public FloatParameter bMultiplier = new FloatParameter { value = 1f };
    [Space]
    [Range(-2f, 200f), Tooltip("X parameter of random value on noise texture.")]
    public FloatParameter x = new FloatParameter { value = 127.1f };
    [Range(-2f, 10002f), Tooltip("Y parameter of random value on noise texture.")]
    public FloatParameter y = new FloatParameter { value = 43758.5453123f };
    [Range(-2f, 200f), Tooltip("Angle Y parameter of random value on noise texture.")]
    public FloatParameter angleY = new FloatParameter { value = 311.7f };
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter();
    public maskChannelModeParameter maskChannel = new maskChannelModeParameter();
}

public sealed class Glitch1Renderer : PostProcessEffectRenderer<RLProGlitch1>
{
     private float T;
    static readonly int _Mask = Shader.PropertyToID("_Mask");
    static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");

    public override void Render(PostProcessRenderContext context)
    {
                    T += Time.deltaTime;
            if (T > 100) T = 0;
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/Glitch1RetroLook"));
        sheet.properties.SetFloat("Strength", settings.amount);

sheet.properties.SetFloat("x", settings.x);
sheet.properties.SetFloat("y", settings.y);
        sheet.properties.SetFloat("angleY", settings.angleY);
        sheet.properties.SetFloat("Stretch", settings.stretch);
        sheet.properties.SetFloat("Speed", settings.speed);
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
        sheet.properties.SetFloat("mR", settings.rMultiplier);
sheet.properties.SetFloat("mG", settings.gMultiplier);
sheet.properties.SetFloat("mB", settings.bMultiplier);

        sheet.properties.SetFloat("Fade", settings.fade);
        sheet.properties.SetFloat("T", T);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }

}