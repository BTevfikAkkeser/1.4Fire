using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using RetroLookPro.Enums;

[Serializable]
[PostProcess(typeof(RLPRO_SRP_OldFilmRenderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Old Film", false)]
public sealed class RLProOldFilm : PostProcessEffectSettings
{
    [Range(0f, 60f), Tooltip("Frames per second.")]
    public FloatParameter fps = new FloatParameter { value = 1f };
    [Range(0f, 5f), Tooltip(".")]
    public FloatParameter contrast = new FloatParameter { value = 1f };

    [Range(-2f, 4f), Tooltip("Image burn.")]
    public FloatParameter burn = new FloatParameter { value = 0.88f };
    [Range(0f, 16f), Tooltip("Scene cut off.")]
    public FloatParameter sceneCut = new FloatParameter { value = 0.88f };
    [Range(0f, 1f), Tooltip(".")]
    public FloatParameter fade = new FloatParameter { value = 0.88f };
    [Space]
    [Tooltip("Mask texture")]
    public TextureParameter mask = new TextureParameter();
    public maskChannelModeParameter maskChannel = new maskChannelModeParameter();

}

public sealed class RLPRO_SRP_OldFilmRenderer : PostProcessEffectRenderer<RLProOldFilm>
{
    private float T;
    static readonly int _Mask = Shader.PropertyToID("_Mask");
    static readonly int _FadeMultiplier = Shader.PropertyToID("_FadeMultiplier");


    public override void Render(PostProcessRenderContext context)
    {
        T += Time.deltaTime;
        if (T > 100) T = 0;

        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/OldFilmFilterRetroLook"));
        sheet.properties.SetFloat("T", T);
        sheet.properties.SetFloat("FPS", settings.fps);
        sheet.properties.SetFloat("Contrast", settings.contrast);
        sheet.properties.SetFloat("Burn", settings.burn);
        sheet.properties.SetFloat("SceneCut", settings.sceneCut);
        sheet.properties.SetFloat("Fade", settings.fade);
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