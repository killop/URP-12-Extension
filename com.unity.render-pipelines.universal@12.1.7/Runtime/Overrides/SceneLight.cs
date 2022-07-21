using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{

    [Serializable, VolumeComponentMenu("Custom/SceneLight")]
    public sealed class SceneLight : VolumeComponent, IPostProcessComponent
    {


        //Ambient

        [Tooltip("AmbientSky")]
        public ColorParameter ambientSky = new ColorParameter(Color.white, true, false, true);

        [Tooltip("AmbientGround")]
        public ColorParameter ambientGround = new ColorParameter(Color.white, true, false, true);

        [Tooltip("AmbientEquator")]
        public ColorParameter ambientEquator = new ColorParameter(Color.white, true, false, true);

        [Tooltip("AmbientCubemap")]
        public CubemapParameter ambientCubemap = new CubemapParameter(null);

        [Tooltip("AmbientCubemapExposure")]
        public MinFloatParameter ambientCubemapExposure = new MinFloatParameter(1f, 0f);


        //Ambient
        [Tooltip("myAmbientSky")]
        public ColorParameter myAmbientSky = new ColorParameter(Color.white, true, false, true);

        [Tooltip("myAmbientGround")]
        public ColorParameter myAmbientGround = new ColorParameter(Color.white, true, false, true);

        [Tooltip("myAmbientLeft")]
        public ColorParameter myAmbientLeft = new ColorParameter(Color.white, true, false, true);

        [Tooltip("myAmbientRight")]
        public ColorParameter myAmbientRight = new ColorParameter(Color.white, true, false, true);

        [Tooltip("myAmbientFront")]
        public ColorParameter myAmbientFront = new ColorParameter(Color.white, true, false, true);

        [Tooltip("myAmbientBack")]
        public ColorParameter myAmbientBack = new ColorParameter(Color.white, true, false, true);

        [Tooltip("mySixSideExposure")]
        public MinFloatParameter mySixSideExposure = new MinFloatParameter(1f, 0f);

        [Tooltip("mySixSideRotateDegree")]
        public ClampedIntParameter mySixSideRotateDegree = new ClampedIntParameter(0, 0, 360);

        [Tooltip("myAmbientCubemap")]
        public CubemapParameter myAmbientCubemap = new CubemapParameter(null);

        [Tooltip("myAmbientCubemapExposure")]
        public MinFloatParameter myAmbientCubemapExposure = new MinFloatParameter(1f, 0f);

        [Tooltip("myAmbientCubemapRotateDegree")]
        public ClampedIntParameter myAmbientCubemapRotateDegree = new ClampedIntParameter(0, 0, 360);


        //fog
        [Tooltip("Fog")]
        public BoolParameter fog = new BoolParameter(false);

        [Tooltip("FogColor")]
        public ColorParameter fogColor = new ColorParameter(Color.white, true, false, true);

        [Tooltip("FogStartDistance")]
        public NoInterpMinFloatParameter startDistance = new NoInterpMinFloatParameter(50f, 0f );

        [Tooltip("FogEndDistance")]
        public NoInterpMinFloatParameter endDistance = new NoInterpMinFloatParameter(70f, 0f);
       

        public bool IsActive() => true;
        
        public bool IsTileCompatible() => false;
    }

   
}


