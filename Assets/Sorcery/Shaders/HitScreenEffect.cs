using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(HitScreenEffectRenderer), PostProcessEvent.AfterStack, "Sorcery/HitEffect")]
public sealed class HitScreenSettings : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("Blood effect intensity.")]
    public FloatParameter blend = new FloatParameter { value = 0.5f };
}

public sealed class HitScreenEffectRenderer : PostProcessEffectRenderer<HitScreenSettings>
{
    public override void Render(PostProcessRenderContext context)
    {
		//Debug.Log("Render " + settings.blend.value);
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Sorcery/BloodOverlay"));
        sheet.properties.SetFloat("_Blend", settings.blend);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}