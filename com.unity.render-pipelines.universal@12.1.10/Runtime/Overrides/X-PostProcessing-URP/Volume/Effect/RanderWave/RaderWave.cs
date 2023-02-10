using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
// #if SOUL_ENGINE
// using SoulEngine;
// #endif

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeDefine.Extra + "探测波 RaderWave")]
    public class RaderWave : VolumeSetting
    {
        public override bool IsActive() => raderWaveStrenght.value > 0;

        [Tooltip("特效强度")]
        public ClampedFloatParameter raderWaveStrenght = new ClampedFloatParameter(0, 0, 1);
        [Tooltip("探测波颜色")]
        public ColorParameter raderWaveColor = new ColorParameter(Color.white);
        [Tooltip("探测范围颜色")]
        public ColorParameter raderAreaColor = new ColorParameter(Color.blue);
        //[Tooltip("探测距离")]
        //public MinFloatParameter raderWaveMaxDistance = new MinFloatParameter(40,0);
        [Tooltip("探测宽度")]
        public ClampedFloatParameter raderWaveWidth = new ClampedFloatParameter(1, 0, 5);

        [Tooltip("传递速度")]
        public MinFloatParameter raderWaveSpeed = new MinFloatParameter(1, 0);
        [Tooltip("传递时间")]
        public MinFloatParameter raderWaveTime = new MinFloatParameter(10, 0);

        private Vector4 m_OriginPos;
        public Vector4 OriginPos { get { return m_OriginPos; } set { m_OriginPos = value; m_TimeReset = true; } }
        private bool m_TimeReset;
        public bool TimeReset { get { return m_TimeReset; } set { m_TimeReset = value; } }

        public override string GetShaderName()
        {
            return "Hidden/PostProcessing/RaderWav";
        }
    }


    public class RaderWaveRenderer : VolumeRenderer<RaderWave>
    {
       
        private float timeCount = 0;
        private const string PROFILER_TAG = "Rader Wave";

        public override bool IsActive()
        {
            if (settings.TimeReset)
            {
                timeCount = 0;
                settings.TimeReset = false;
            }

            if (timeCount > settings.raderWaveTime.value)
            {
                return false;
            }
            else
            {
                timeCount += Time.deltaTime;
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



            cmd.BeginSample(PROFILER_TAG);
            // #if SOUL_ENGINE     
            // 		    // IKernel kernel = GameWorld.gameKernel;	
            // 			// Vector4 vec = LocalHelper.GetPosition(kernel,GameWorld.gameKernel.GetMainPlayer());				 
            // 			// vec.w = timeCount * settings.raderWaveSpeed.value;
            // 			// m_BlitMaterial.SetVector(ShaderContants.raderOriginPosID,vec);
            // #endif
            Vector4 vec = settings.OriginPos;
            vec.w = timeCount * settings.raderWaveSpeed.value;
            m_BlitMaterial.SetVector(ShaderContants.raderOriginPosID, vec);
            m_BlitMaterial.SetVector(ShaderContants.raderWaveColorID, settings.raderWaveColor.value);
            //m_BlitMaterial.SetFloat(ShaderContants.raderMaxDistanceID,settings.raderWaveMaxDistance.value);
            m_BlitMaterial.SetFloat(ShaderContants.raderWaveWidthID, settings.raderWaveWidth.value);
            m_BlitMaterial.SetVector(ShaderContants.raderWaveAreaColorID, settings.raderAreaColor.value);

            cmd.Blit(source, target, m_BlitMaterial);
            //cmd.Blit(target,source);

            cmd.EndSample(PROFILER_TAG);
        }
    }



}