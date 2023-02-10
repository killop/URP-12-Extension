using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
// #if SOUL_ENGINE
// using SoulEngine;
// #endif

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Extra + " Èí¾Û½¹")]
    public class SingleFocusSoft : VolumeSetting
    {
        public override bool IsActive() => transitioning.value;



        /// <summary>
        /// Background color that will be used during transition.
        /// </summary>
        [Tooltip("Background color that will be used during transition.")]
        public ColorParameter backgroundColor = new ColorParameter(Color.black);

        /// <summary>
        /// Texture that will be used as background during transition.
        /// Render Texture also allowed.
        /// </summary>
        [Tooltip("Texture that will be used as background during transition. Render Texture also allowed.")]
        public Texture2DParameter  backgroundTexture = new Texture2DParameter(null);

        public enum BackgroundType
        {
            COLOR,
            TEXTURE
        }
        /// <summary>
        /// Defines what type background will be used during transition.
        /// </summary>
        [Tooltip("Defines what type background will be used during transition.")]
        public NoInterpIntParameter backgroundType = new NoInterpIntParameter(0);



        /// <summary>
		/// Represents current progress of the transition.
		/// 0 - no transition
		/// from 1.5 to 2.5 - full transition to background color (depends on the falloff).
		/// </summary>
		[Range(0f, 2.5f), Tooltip("Represents current progress of the transition.")]
        public ClampedFloatParameter cutoff = new ClampedFloatParameter(0,0,2.5f);

        /// <summary>
        /// Smooth blend between rendered texture and background color.
        /// 0 - no blend (sharp border)
        /// 1 - max blend 
        /// </summary>
        [Range(0f, 1f), Tooltip("Smooth blend between rendered texture and background color.")]
        public ClampedFloatParameter falloff = new ClampedFloatParameter(0, 0, 1f);
        /// <summary>
        /// Flag that tells Unity to process transition. 
        /// Set this flag at the beginning of the transition and unset at the end 
        /// to avoid unnecessary calculations and save some performance.
        /// </summary>
        [Tooltip("Flag that tells Unity to process transition. Set this flag at the beginning of the transition and unset it at the end to avoid unnecessary calculations and to save some performance.")]
        public BoolParameter transitioning = new BoolParameter(true);

        /// <summary>
        /// Position of this Transform will be the center of the transition's circle.
        /// When Transform is Null or placed behind the camera, center of the screen will be used as the target instead.
        /// </summary>
        [Tooltip("Position of this Transform will be the center of the transition's circle. When Transform is Null or placed behind the camera, center of the screen will be used as the target instead.")]
        public Transform focus = null;

        /// <summary>
        /// camera
        /// </summary>
        [Tooltip("Camera.")]
        public Camera camera = null;

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/RaderWav";
        }
    }


    public class SingleFocusSoftRenderer : VolumeRenderer<SingleFocusSoft>
    {

        private const string PROFILER_TAG = "SingleFocusSoft";

        public override bool IsActive()
        {
            if (settings.camera == null)
            {
                return false;
            }
            return base.IsActive();
        }


        static class ShaderContants
        {
            public static readonly int raderOriginPosID = Shader.PropertyToID("_OriginPosition");
            public static readonly int raderWaveColorID = Shader.PropertyToID("_WaveColor");
            public static readonly int raderMaxDistanceID = Shader.PropertyToID("_MaxDistance");
            public static readonly int raderWaveWidthID = Shader.PropertyToID("_RaderWaveWidth");
            public static readonly int raderWaveAreaColorID = Shader.PropertyToID("_RaderAreaColor");
        }

        public override void Render(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if (m_BlitMaterial == null) return;
           
            m_BlitMaterial.SetFloat("_Cutoff",settings.cutoff.value);
            m_BlitMaterial.SetFloat("_Falloff", settings.falloff.value);

            switch (settings.backgroundType.value)
            {
                case 0:
                    m_BlitMaterial.DisableKeyword("USE_TEXTURE");
                    m_BlitMaterial.SetColor("_Color", settings.backgroundColor.value);
                    break;
                case 1:
                    m_BlitMaterial.EnableKeyword("USE_TEXTURE");
                    m_BlitMaterial.SetTexture("_Texture", settings.backgroundTexture.value);
                    break;
            }
            var focus = settings.focus;
            if (focus != null)
            {
                var camera = settings.camera;
                var transform = camera.transform;
                // Check if the focus Transform is behind the camera
                Vector3 dir = (focus.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, dir);

                if (dot > 0)
                {
                    Vector2 screenPos = camera.WorldToViewportPoint(focus.position);
                    m_BlitMaterial.SetFloat("_FocusX", screenPos.x);
                    m_BlitMaterial.SetFloat("_FocusY", screenPos.y);
                }
            }
            else 
            {
                m_BlitMaterial.SetFloat("_FocusX", 0.5f);
                m_BlitMaterial.SetFloat("_FocusY", 0.5f);
            }
            cmd.Blit(source, target, m_BlitMaterial);
            cmd.EndSample(PROFILER_TAG);
        }
    }



}