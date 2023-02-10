using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Pixelate + "六边格像素化 (PixelizeHexagonGrid)")]
    public class PixelizeHexagonGrid : VolumeSetting
    {
        public override bool IsActive() => pixelSize.value > 0;

        public FloatParameter pixelSize = new ClampedFloatParameter(0f, 0.01f, 1.0f);
        public FloatParameter gridWidth = new ClampedFloatParameter(1.0f, 0.01f, 5.0f);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Pixelate/PixelizeHexagonGrid";
        }
    }

    public class PixelizeHexagonGridRenderer : VolumeRenderer<PixelizeHexagonGrid>
    {
        private const string PROFILER_TAG = "PixelizeHexagonGrid";
       

        static class ShaderIDs
        {
            internal static readonly int Params = Shader.PropertyToID("_Params");
        }


        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            m_BlitMaterial.SetVector(ShaderIDs.Params, new Vector2(settings.pixelSize.value, settings.gridWidth.value));

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}