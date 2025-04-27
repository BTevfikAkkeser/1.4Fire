using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(RLPRO_SRP_EdgeStretch_Renderer), PostProcessEvent.BeforeStack, "Retro Look Pro/Edge Stretch Effect", false)]
public sealed class RLProEdgeStretch : PostProcessEffectSettings
{
    public BoolParameter left = new BoolParameter { value = true };
    public BoolParameter right = new BoolParameter { value = true };
    public BoolParameter top = new BoolParameter { value = true };
    public BoolParameter bottom = new BoolParameter { value = true };
    [Range(0.01f, 0.5f), Tooltip("Height of Noise.")]
    public FloatParameter height = new FloatParameter { value = 0.2f };
    [Range(0f, 50f), Tooltip("speed of Noise.")]
    public FloatParameter speed = new FloatParameter { value = 0.2f };

    [Space]
            [ Tooltip("distort stretched area.")]
    public BoolParameter distort = new BoolParameter { value = true };
        [Range(0.1f, 100f), Tooltip("Distortion frequency.")]
    public FloatParameter frequency = new FloatParameter { value = 0.2f };
        [Range(0f, 0.5f), Tooltip("Distortion amplitude.")]
    public FloatParameter amplitude = new FloatParameter { value = 0.2f };

            [ Tooltip("enable random amplitude and frequency.")]
    public BoolParameter distortRandomly = new BoolParameter { value = true };

}
public sealed class RLPRO_SRP_EdgeStretch_Renderer : PostProcessEffectRenderer<RLProEdgeStretch>
{
	private float T;
    public override void Render(PostProcessRenderContext context)
    {
		T += Time.deltaTime;
        var sheet = context.propertySheets.Get(Shader.Find("RetroLookPro/EdgeStretchEffect"));
		sheet.properties.SetFloat("Time", T);
        sheet.properties.SetFloat("_NoiseBottomHeight", settings.height);
        sheet.properties.SetFloat("frequency", settings.frequency);
        sheet.properties.SetFloat("amplitude", settings.amplitude);
        sheet.properties.SetFloat("speed", settings.speed);
        ParamSwitch(sheet, settings.top.value, "top_ON");
        ParamSwitch(sheet, settings.bottom.value, "bottom_ON");
        ParamSwitch(sheet, settings.left.value, "left_ON");
        ParamSwitch(sheet, settings.right.value, "right_ON");

        if (settings.distort){
if(settings.distortRandomly)
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 1);
        else
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
        else
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 2);
    }
    private void ParamSwitch(PropertySheet mat, bool paramValue, string paramName)
    {
        if (paramValue) mat.EnableKeyword(paramName);
        else mat.DisableKeyword(paramName);
    }

}
