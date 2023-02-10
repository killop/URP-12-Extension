using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.EdgeDetection + "ScharrNeonV2")]
    public class EdgeDetectionScharrNeonV2 : VolumeSetting
    {
        public override bool IsActive() => EdgeWidth.value > 0;

        public FloatParameter EdgeWidth = new ClampedFloatParameter(0f, 0.05f, 5.0f);
        public FloatParameter EdgeNeonFade = new ClampedFloatParameter(1f, 0.1f, 1.0f);
        public FloatParameter BackgroundFade = new ClampedFloatParameter(1f, 0f, 1f);
        public FloatParameter Brigtness = new ClampedFloatParameter(1f, 0.2f, 2.0f);
        public ColorParameter BackgroundColor = new ColorParameter(Color.black, true, true, true);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/EdgeDetection/ScharrNeonV2";
        }
    }

    public class EdgeDetectionScharrNeonV2Renderer : VolumeRenderer<EdgeDetectionScharrNeonV2>
    {
        private const string PROFILER_TAG = "EdgeDetectionScharrNeonV2";
     

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int BackgroundColor = Shader.PropertyToID("_BackgroundColor");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);


            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.EdgeWidth.value, settings.EdgeNeonFade.value, settings.Brigtness.value, settings.BackgroundFade.value));
            m_BlitMaterial.SetColor(ShaderIDs.BackgroundColor, settings.BackgroundColor.value);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}