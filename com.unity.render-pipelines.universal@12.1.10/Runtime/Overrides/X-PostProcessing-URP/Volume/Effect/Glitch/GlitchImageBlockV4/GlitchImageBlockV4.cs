using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "错位图块故障V4 (Image Block GlitchV4)")]
    public class GlitchImageBlockV4 : VolumeSetting
    {
        public override bool IsActive() => Speed.value > 0;

        public FloatParameter Speed = new ClampedFloatParameter(0f, 0f, 50f);
        public FloatParameter BlockSize = new ClampedFloatParameter(8f, 0f, 50f);
        public FloatParameter MaxRGBSplitX = new ClampedFloatParameter(1f, 0f, 25f);
        public FloatParameter MaxRGBSplitY = new ClampedFloatParameter(1f, 0f, 25f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/ImageBlockV4";
        }
    }

    public class GlitchImageBlockV4Renderer : VolumeRenderer<GlitchImageBlockV4>
    {
        private const string PROFILER_TAG = "GlitchImageBlockV4";
       

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");

        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;


            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(settings.Speed.value, settings.BlockSize.value, settings.MaxRGBSplitX.value, settings.MaxRGBSplitY.value));

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}