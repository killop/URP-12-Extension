using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Linq;

namespace XPostProcessing
{
    public class CustomPostProcess
    {

        const string k_RenderPostProcessingTag = "Custom Render PostProcessing Effects";
       // const string k_RenderFinalPostProcessingTag = "Custom Render Final PostProcessing Pass";
        private static readonly ProfilingSampler m_ProfilingRenderPostProcessing = new ProfilingSampler(k_RenderPostProcessingTag);
       // private static readonly ProfilingSampler m_ProfilingRenderFinalPostProcessing = new ProfilingSampler(k_RenderFinalPostProcessingTag);

        List<AbstractVolumeRenderer> m_PostProcessingRenderers = new List<AbstractVolumeRenderer>();
        List<AbstractVolumeRenderer> m_AcitvePostProcessing = null;

        //RenderTargetIdentifier m_Source;
        //RenderTargetHandle m_TempRT0;
        //RenderTargetHandle m_TempRT1;
        //RenderTextureDescriptor m_Descriptor;


       // public int downSample;

        public CustomPostProcess()
        {
           // m_TempRT0.Init("_TempRT0");
           // m_TempRT1.Init("_TempRT1");

            // var customVolumes = VolumeManager.instance.baseComponentTypeArray
            //     .Where(t => t.IsSubclassOf(typeof(VolumeSetting)))
            //     .Select(t => VolumeManager.instance.stack.GetComponent(t) as VolumeSetting)
            //     .ToList();
            // Debug.LogError("customVolumesL:" + customVolumes.Count);

            // for (int i = 0; i < customVolumes.Count; i++)
            // {
            //     AddEffect(customVolumes[i]);
            // }

            AddEffect(new DepthFogRenderer());
            AddEffect(new CloudShadowRenderer());
            //故障
            AddEffect(new GlitchRGBSplitRenderer());
            AddEffect(new GlitchRGBSplitV2Renderer());
            AddEffect(new GlitchRGBSplitV3Renderer());
            AddEffect(new GlitchRGBSplitV4Renderer());
            AddEffect(new GlitchRGBSplitV5Renderer());
            AddEffect(new GlitchDigitalStripeRenderer());
            AddEffect(new GlitchImageBlockRenderer());
            AddEffect(new GlitchImageBlockV2Renderer());
            AddEffect(new GlitchImageBlockV3Renderer());
            AddEffect(new GlitchImageBlockV4Renderer());
            AddEffect(new GlitchLineBlockRenderer());
            AddEffect(new GlitchTileJitterRenderer());
            AddEffect(new GlitchScanLineJitterRenderer());
            AddEffect(new GlitchAnalogNoiseRenderer());
            AddEffect(new GlitchScreenJumpRenderer());
            AddEffect(new GlitchScreenShakeRenderer());
            AddEffect(new GlitchWaveJitterRenderer());
            //边缘检测
            AddEffect(new EdgeDetectionRobertsRenderer());
            AddEffect(new EdgeDetectionRobertsNeonRenderer());
            AddEffect(new EdgeDetectionRobertsNeonV2Renderer());
            AddEffect(new EdgeDetectionScharrRenderer());
            AddEffect(new EdgeDetectionScharrNeonRenderer());
            AddEffect(new EdgeDetectionScharrNeonV2Renderer());
            AddEffect(new EdgeDetectionSobelRenderer());
            AddEffect(new EdgeDetectionSobelNeonRenderer());
            AddEffect(new EdgeDetectionSobelNeonV2Renderer());
            //像素化
            AddEffect(new PixelizeCircleRenderer());
            AddEffect(new PixelizeDiamondRenderer());
            AddEffect(new PixelizeHexagonRenderer());
            AddEffect(new PixelizeHexagonGridRenderer());
            AddEffect(new PixelizeLeafRenderer());
            AddEffect(new PixelizeLedRenderer());
            AddEffect(new PixelizeQuadRenderer());
            AddEffect(new PixelizeSectorRenderer());
            AddEffect(new PixelizeTriangleRenderer());


            AddEffect(new IrisBlurRenderer());
            AddEffect(new RainRippleRenderer());

            AddEffect(new SurfaceSnowRenderer());
            AddEffect(new RaderWaveRenderer());
            AddEffect(new BulletTimeRenderer());

            //Blur
            AddEffect(new GaussianBlurRenderer());
            AddEffect(new BoxBlurRenderer());
            AddEffect(new KawaseBlurRenderer());
            AddEffect(new DualBoxBlurRenderer());
            AddEffect(new DualGaussianBlurRenderer());
            AddEffect(new DualKawaseBlurRenderer());
            AddEffect(new DualTentBlurRenderer());
            AddEffect(new BokehBlurRenderer());
            AddEffect(new TiltShiftBlurRenderer());
            AddEffect(new IrisBlurRenderer());
            AddEffect(new IrisBlurV2Renderer());
            AddEffect(new GrainyBlurRenderer());
            AddEffect(new RadialBlurRenderer());
            AddEffect(new RadialBlurV2Renderer());
            AddEffect(new DirectionalBlurRenderer());

            //Image Processing
            AddEffect(new SharpenV1Renderer());
            AddEffect(new SharpenV2Renderer());
            AddEffect(new SharpenV3Renderer());


            //Vignette
            AddEffect(new RapidOldTVVignetteRenderer());
            AddEffect(new RapidOldTVVignetteV2Renderer());
            AddEffect(new RapidVignetteRenderer());
            AddEffect(new RapidVignetteV2Renderer());
            AddEffect(new AuroraVignetteRenderer());

            //色彩调整
            AddEffect(new ColorAdjustmentBleachBypassRenderer());
            AddEffect(new ColorAdjustmentBrightnessRenderer());
            AddEffect(new ColorAdjustmentContrastRenderer());
            AddEffect(new ColorAdjustmentContrastV2Renderer());
            AddEffect(new ColorAdjustmentContrastV3Renderer());
            AddEffect(new ColorAdjustmentHueRenderer());
            AddEffect(new ColorAdjustmentLensFilterRenderer());
            AddEffect(new ColorAdjustmentSaturationRenderer());
            AddEffect(new ColorAdjustmentTechnicolorRenderer());
            AddEffect(new ColorAdjustmentTintRenderer());
            AddEffect(new ColorAdjustmentWhiteBalanceRenderer());
            AddEffect(new ColorAdjustmentColorReplaceRenderer());
            AddEffect(new ScreenBinarizationRenderer());

            m_AcitvePostProcessing = new List<AbstractVolumeRenderer>(m_PostProcessingRenderers.Count);
        }

        private void AddEffect(AbstractVolumeRenderer renderer)
        {
            m_PostProcessingRenderers.Add(renderer);
            renderer.Init();
        }

        public void Cleanup()
        {
            for (int i = 0; i < m_PostProcessingRenderers.Count; i++)
            {
                m_PostProcessingRenderers[i].Cleanup();
            }
            m_PostProcessingRenderers = new List<AbstractVolumeRenderer>();
            m_AcitvePostProcessing = null;
        }

        //public void Setup(RenderTargetIdentifier source)
        //{
        //    m_Source = source;
        //    // m_Destination = destination;
        //}
        //
        //public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        //{
        //    RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        //    m_Descriptor = cameraTargetDescriptor;
        //    m_Descriptor.msaaSamples = 1;
        //    m_Descriptor.depthBufferBits = 32;
        //    m_Descriptor.width = m_Descriptor.width >> downSample;
        //    m_Descriptor.height = m_Descriptor.height >> downSample;
        //
        //
        //    cmd.GetTemporaryRT(m_TempRT0.id, m_Descriptor);
        //    cmd.GetTemporaryRT(m_TempRT1.id, m_Descriptor);
        //
        //    ConfigureTarget(m_TempRT0.Identifier());
        //    ConfigureClear(ClearFlag.None, Color.white);
        //}
        //
        //public override void OnCameraCleanup(CommandBuffer cmd)
        //{
        //
        //    cmd.ReleaseTemporaryRT(m_TempRT0.id);
        //    cmd.ReleaseTemporaryRT(m_TempRT1.id);
        //}

        public List<AbstractVolumeRenderer> GetAcitvePostProcessingRenderers()
        {
            m_AcitvePostProcessing.Clear();
            foreach (var renderer in m_PostProcessingRenderers)
            {
                if (renderer.IsActive())
                {
                    m_AcitvePostProcessing.Add(renderer);
                }
            }
            return m_AcitvePostProcessing;
        }

        //public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        //{
        //    // if (!Application.isPlaying) return;
        //
        //    var cmd = CommandBufferPool.Get();
        //    cmd.Clear();
        //
        //   //// 初始化临时RT
        //   //RenderTargetIdentifier buff0, buff1;
        //   //buff0 = m_TempRT0.id;
        //   //buff1 = m_TempRT1.id;
        //   //RenderTargetIdentifier GetSource() => buff0;
        //   //RenderTargetIdentifier GetTarget() => buff1;
        //
        //    void Swap() => CoreUtils.Swap(ref buff0, ref buff1);
        //    using (new ProfilingScope(cmd, m_ProfilingRenderPostProcessing))
        //    {
        //        Blit(cmd, m_Source, buff0);
        //        foreach (var renderer in m_PostProcessingRenderers)
        //        {
        //            if (renderer.IsActive())
        //            {
        //                renderer.Render(cmd, GetSource(), GetTarget());
        //                Swap();
        //            }
        //        }
        //
        //        Blit(cmd, buff0, m_Source);
        //    }
        //
        //
        //    context.ExecuteCommandBuffer(cmd);
        //    CommandBufferPool.Release(cmd);
        //}

    }
}