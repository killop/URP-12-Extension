using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Pixelate + "叶子像素化 (PixelizeLeaf)")]
    public class PixelizeLeaf : VolumeSetting
    {
        public override bool IsActive() => pixelSize.value > 0;
        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.01f, 1.0f);
        public BoolParameter useAutoScreenRatio = new BoolParameter(false);
        public FloatParameter pixelRatio = new ClampedFloatParameter(1f, 0.2f, 5.0f);

        [Tooltip("像素缩放X")]
        public FloatParameter pixelScaleX = new ClampedFloatParameter(1f, 0.2f, 5.0f);

        [Tooltip("像素缩放Y")]
        public FloatParameter pixelScaleY = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Pixelate/PixelizeLeaf";
        }
    }

    public class PixelizeLeafRenderer : VolumeRenderer<PixelizeLeaf>
    {
        private const string PROFILER_TAG = "PixelizeLeaf";
      

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            float size = (1.01f - settings.pixelSize.value) * 10f;

            float ratio = settings.pixelRatio.value;
            if (settings.useAutoScreenRatio.value)
            {
                ratio = (float)(Screen.width / (float)Screen.height);
                if (ratio == 0)
                {
                    ratio = 1f;
                }
            }

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector4(size, ratio, settings.pixelScaleX.value * 20, settings.pixelScaleY.value * 20));


            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}