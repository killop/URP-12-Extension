using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Pixelate + "扇形像素化 (PixelizeSector)")]
    public class PixelizeSector : VolumeSetting
    {
        public override bool IsActive() => pixelSize.value > 0;

        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.01f, 1.0f);
        public FloatParameter circleRadius = new ClampedFloatParameter(0.8f, 0.01f, 1.0f);

        [Tooltip("像素缩放X")]
        public FloatParameter pixelIntervalX = new ClampedFloatParameter(1f, 0.2f, 5.0f);

        [Tooltip("像素缩放Y")]
        public FloatParameter pixelIntervalY = new ClampedFloatParameter(1f, 0.2f, 5.0f);
        public ColorParameter BackgroundColor = new ColorParameter(Color.black, true, true, true);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Pixelate/PixelizeSector";
        }
    }

    public class PixelizeSectorRenderer : VolumeRenderer<PixelizeSector>
    {
        private const string PROFILER_TAG = "PixelizeSector";
       


       

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
            internal static readonly int Params2 = Shader.PropertyToID("_Params2");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            float size = (1.01f - settings.pixelSize.value) * 300f;
            Vector4 parameters = new Vector4(size, ((Screen.width * 2 / Screen.height) * size / Mathf.Sqrt(3f)), settings.circleRadius.value, 0f);

            m_BlitMaterial.SetVector(ShaderIDs.Params, parameters);
            m_BlitMaterial.SetVector(ShaderIDs.Params2, new Vector2(settings.pixelIntervalX.value, settings.pixelIntervalY.value));
            m_BlitMaterial.SetColor("_BackgroundColor", settings.BackgroundColor.value);

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}