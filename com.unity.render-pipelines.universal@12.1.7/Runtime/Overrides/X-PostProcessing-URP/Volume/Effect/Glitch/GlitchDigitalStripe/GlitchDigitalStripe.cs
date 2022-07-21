using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Glitch + "数字条纹故障 (Digital Stripe Glitch)")]
    public class GlitchDigitalStripe : VolumeSetting
    {
        public override bool IsActive() => intensity.value > 0;

        public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
        public ClampedIntParameter frequncy = new ClampedIntParameter(3, 1, 10);
        public ClampedFloatParameter stripeLength = new ClampedFloatParameter(0.89f, 0f, 0.99f);
        public ClampedIntParameter noiseTextureWidth = new ClampedIntParameter(20, 8, 256);


        public ClampedIntParameter noiseTextureHeight = new ClampedIntParameter(20, 8, 256);

        public BoolParameter needStripColorAdjust = new BoolParameter(false);
        public ColorParameter StripColorAdjustColor = new ColorParameter(new Color(0.1f, 0.1f, 0.1f), true, true, true);

        public ClampedFloatParameter StripColorAdjustIndensity = new ClampedFloatParameter(2, 0, 10);

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/Glitch/DigitalStripe";
        }
    }

    public class GlitchDigitalStripeRenderer : VolumeRenderer<GlitchDigitalStripe>
    {
        private const string PROFILER_TAG = "GlitchDigitalStripe";
      

        Texture2D _noiseTexture;
        RenderTexture _trashFrame1;
        RenderTexture _trashFrame2;


        static class ShaderIDs
        {
            internal static readonly int indensity = Shader.PropertyToID("_Indensity");
            internal static readonly int noiseTex = Shader.PropertyToID("_NoiseTex");
            internal static readonly int StripColorAdjustColor = Shader.PropertyToID("_StripColorAdjustColor");
            internal static readonly int StripColorAdjustIndensity = Shader.PropertyToID("_StripColorAdjustIndensity");
        }

        void UpdateNoiseTexture(int frame, int noiseTextureWidth, int noiseTextureHeight, float stripLength)
        {
            int frameCount = Time.frameCount;
            if (frameCount % frame != 0)
            {
                return;
            }

            _noiseTexture = new Texture2D(noiseTextureWidth, noiseTextureHeight, TextureFormat.ARGB32, false);
            _noiseTexture.wrapMode = TextureWrapMode.Clamp;
            _noiseTexture.filterMode = FilterMode.Point;

            _trashFrame1 = new RenderTexture(Screen.width, Screen.height, 0);
            _trashFrame2 = new RenderTexture(Screen.width, Screen.height, 0);
            _trashFrame1.hideFlags = HideFlags.DontSave;
            _trashFrame2.hideFlags = HideFlags.DontSave;

            Color32 color = VolumeUtility.RandomColor();

            for (int y = 0; y < _noiseTexture.height; y++)
            {
                for (int x = 0; x < _noiseTexture.width; x++)
                {
                    //随机值若大于给定strip随机阈值，重新随机颜色
                    if (UnityEngine.Random.value > stripLength)
                    {
                        color = VolumeUtility.RandomColor();
                    }
                    //设置贴图像素值
                    _noiseTexture.SetPixel(x, y, color);
                }
            }

            _noiseTexture.Apply();

            var bytes = _noiseTexture.EncodeToPNG();
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {

            if (m_BlitMaterial == null)
                return;

            cmd.BeginSample(PROFILER_TAG);

            UpdateNoiseTexture(settings.frequncy.value, settings.noiseTextureWidth.value, settings.noiseTextureHeight.value, settings.stripeLength.value);

            m_BlitMaterial.SetFloat(ShaderIDs.indensity, settings.intensity.value);

            if (_noiseTexture != null)
            {
                m_BlitMaterial.SetTexture(ShaderIDs.noiseTex, _noiseTexture);
            }

            if (settings.needStripColorAdjust == true)
            {
                m_BlitMaterial.EnableKeyword("NEED_TRASH_FRAME");
                m_BlitMaterial.SetColor(ShaderIDs.StripColorAdjustColor, settings.StripColorAdjustColor.value);
                m_BlitMaterial.SetFloat(ShaderIDs.StripColorAdjustIndensity, settings.StripColorAdjustIndensity.value);
            }
            else
            {
                m_BlitMaterial.DisableKeyword("NEED_TRASH_FRAME");
            }

            cmd.Blit(source, target, m_BlitMaterial);

            cmd.EndSample(PROFILER_TAG);
        }
    }

}