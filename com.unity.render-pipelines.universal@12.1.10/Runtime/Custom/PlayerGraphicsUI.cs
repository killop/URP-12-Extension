using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGraphicsUI : MonoBehaviour
{ 
    public enum FrameRate
    {
        Low = 30,
        Med = 40,
        High = 60
    }
    public enum ShaderLod
    {
        Low = 300,
        High = 500
    }
    public static bool ShowEnable= true;
    public GameObject ShowGameObject = null;
    //分辨率设置
    public ResolutionLevel resolutionLevel = ResolutionLevel.HIGH;

    public Text frameRateText;
    public Text shaderLodText;
    public Text resolutionLevelText;

    public Toggle displayFPSToggle;
    public Toggle dynamicQualityToggle;
    public Toggle hdrToggle;
    public Toggle shadowToggle;
    public Toggle playerBloomToggle;
    public Toggle alphaBufferToggle;
    public Toggle alphaBloomToggle;
    public Toggle sceneBloomToggle;
    public Toggle antialiasingToggle;
    public Toggle depthOfFieldToggle;
    public Toggle ambientOcclusionToggle;
    public Toggle volumetricFogToggle;
    public Toggle LowShadowToggle;
    public Toggle radialBlurToggle;
    public Toggle cloudShadowToggle;
    public Toggle heightFogToggle;
    public Toggle chromaticAberrationToggle;
    public Toggle csPointLightToggle;
    public Toggle reflectionMapToggle;
    public Toggle splitUIRenderScaleToggle;

    //public Toggle playerLayerToggle;
    //public Toggle distortionToggle;
    //public Toggle borderToggle;

    private PlayerGraphicsSetting graphicsSetting;
    private PlayerGraphicsLogic logic;

    GUIStyle style;

    // Start is called before the first frame update
    void Start()
    {
        if (ShowGameObject)
        {
            ShowGameObject.SetActive(ShowEnable);
        }
        graphicsSetting = GetComponentInParent<PlayerGraphicsSetting>();
        logic = GetComponentInParent<PlayerGraphicsLogic>();
        if (graphicsSetting)
        {
            GetSetting();
        }

        style = new GUIStyle();
        style.normal.background = null;    //这是设置背景填充的
        style.normal.textColor = new Color(1, 1, 1);   //设置字体颜色的
        style.fontSize = 40;       //当然，这是字体颜色

        Refresh();
    }

    public void OnFrameRateButtonClick()
    {
        if (graphicsSetting.frameRate > 59)
            graphicsSetting.frameRate = 30;
        else if(graphicsSetting.frameRate > 35)
            graphicsSetting.frameRate = 60;
        else 
            graphicsSetting.frameRate = 45;

        frameRateText.text = "FrameRate:" + graphicsSetting.frameRate;

        Refresh();
    }
    public void OnShaderLodClick()
    {
        if (graphicsSetting.shaderLod > 400)
            graphicsSetting.shaderLod = 300;     
        else
            graphicsSetting.shaderLod = 500;

        shaderLodText.text = "ShaderLod:" + graphicsSetting.shaderLod;

        Refresh();
    }
    public void OnResolutionLevelClick()
    {
        if (graphicsSetting.resolutionLevel < ResolutionLevel.VERYLOW)
            graphicsSetting.resolutionLevel += 1;
        else
            graphicsSetting.resolutionLevel = 0;

        resolutionLevelText.text = "ResolutionLevel:" + graphicsSetting.resolutionLevel;       

        Refresh();
    }

    void GetSetting()
    {
        frameRateText.text = "FrameRate:" + graphicsSetting.frameRate;
        shaderLodText.text = "ShaderLod:" + graphicsSetting.shaderLod;
        resolutionLevelText.text = "ResolutionLevel:" + graphicsSetting.resolutionLevel;

        displayFPSToggle.isOn = graphicsSetting.displayFPS;
        dynamicQualityToggle.isOn =  graphicsSetting.dynamicQuality;
        hdrToggle.isOn =  graphicsSetting.hdr;
        shadowToggle.isOn =  graphicsSetting.shadow;
        //playerLayerToggle.isOn =  graphicsSetting.playerLayer;
        playerBloomToggle.isOn =  graphicsSetting.playerBloom;
        alphaBufferToggle.isOn =  graphicsSetting.alphaBuffer;
        //distortionToggle.isOn =  graphicsSetting.distortion;
        alphaBloomToggle.isOn =  graphicsSetting.alphaBloom;
        sceneBloomToggle.isOn =  graphicsSetting.sceneBloom;
        antialiasingToggle.isOn =  graphicsSetting.antialiasing;
        depthOfFieldToggle.isOn =  graphicsSetting.depthOfField;
        ambientOcclusionToggle.isOn =  graphicsSetting.ambientOcclusion;
        volumetricFogToggle.isOn =  graphicsSetting.volumetricFog;
        //borderToggle.isOn =  graphicsSetting.border;
        LowShadowToggle.isOn =  graphicsSetting.border;
        radialBlurToggle.isOn =  graphicsSetting.radialBlur;
        cloudShadowToggle.isOn =  graphicsSetting.cloudShadow;
        heightFogToggle.isOn =  graphicsSetting.heightFog;
        chromaticAberrationToggle.isOn =  graphicsSetting.chromaticAberration;
        csPointLightToggle.isOn = graphicsSetting.csPointLight;
        reflectionMapToggle.isOn = graphicsSetting.reflectionMap;
       // splitUIRenderScaleToggle.isOn = UnityEngine.Rendering.Universal.UniversalRenderer.sUISplitEnable;
    }

    void SetSetting()
    {
        graphicsSetting.displayFPS = displayFPSToggle.isOn;
        graphicsSetting.dynamicQuality = dynamicQualityToggle.isOn;
        graphicsSetting.hdr = hdrToggle.isOn;
        graphicsSetting.shadow = shadowToggle.isOn;
        //graphicsSetting.playerLayer = playerLayerToggle.isOn;
        graphicsSetting.playerBloom = playerBloomToggle.isOn;
        graphicsSetting.alphaBuffer = alphaBufferToggle.isOn;
        //graphicsSetting.distortion = distortionToggle.isOn;
        graphicsSetting.alphaBloom = alphaBloomToggle.isOn;
        graphicsSetting.sceneBloom = sceneBloomToggle.isOn;
        graphicsSetting.antialiasing = antialiasingToggle.isOn;
        graphicsSetting.depthOfField = depthOfFieldToggle.isOn;
        graphicsSetting.ambientOcclusion = ambientOcclusionToggle.isOn;
        graphicsSetting.volumetricFog = volumetricFogToggle.isOn;
        //graphicsSetting.border = borderToggle.isOn;
        graphicsSetting.lowShadow = LowShadowToggle.isOn;
        graphicsSetting.radialBlur = radialBlurToggle.isOn;
        graphicsSetting.cloudShadow = cloudShadowToggle.isOn;
        graphicsSetting.heightFog = heightFogToggle.isOn;
        graphicsSetting.chromaticAberration = chromaticAberrationToggle.isOn;
        graphicsSetting.csPointLight = csPointLightToggle.isOn;
        graphicsSetting.reflectionMap = reflectionMapToggle.isOn;
      //  Debug.LogError("ui"+splitUIRenderScaleToggle.isOn);
       // UnityEngine.Rendering.Universal.UniversalRenderer.sUISplitEnable = splitUIRenderScaleToggle.isOn;

        graphicsSetting.Save();
    }


    // Update is called once per frame
    void Update()
    {
        if (graphicsSetting == null)
            return;

        if (graphicsSetting.displayFPS != displayFPSToggle.isOn ||
           graphicsSetting.dynamicQuality != dynamicQualityToggle.isOn ||
           graphicsSetting.hdr != hdrToggle.isOn ||
           graphicsSetting.shadow != shadowToggle.isOn ||
           // graphicsSetting.playerLayer != playerLayerToggle.isOn ||
           graphicsSetting.playerBloom != playerBloomToggle.isOn ||
           graphicsSetting.alphaBuffer != alphaBufferToggle.isOn ||
           //graphicsSetting.distortion != distortionToggle.isOn ||
           graphicsSetting.alphaBloom != alphaBloomToggle.isOn ||
           graphicsSetting.sceneBloom != sceneBloomToggle.isOn ||
           graphicsSetting.antialiasing != antialiasingToggle.isOn ||
           graphicsSetting.depthOfField != depthOfFieldToggle.isOn ||
           graphicsSetting.ambientOcclusion != ambientOcclusionToggle.isOn ||
           graphicsSetting.volumetricFog != volumetricFogToggle.isOn ||
           //graphicsSetting.border != borderToggle.isOn ||
           graphicsSetting.lowShadow != LowShadowToggle.isOn ||
           graphicsSetting.radialBlur != radialBlurToggle.isOn ||
           graphicsSetting.cloudShadow != cloudShadowToggle.isOn ||
           graphicsSetting.heightFog != heightFogToggle.isOn ||
           graphicsSetting.chromaticAberration != chromaticAberrationToggle.isOn ||
           graphicsSetting.csPointLight != csPointLightToggle.isOn || 
           graphicsSetting.reflectionMap != reflectionMapToggle.isOn)
        //  UnityEngine.Rendering.Universal.UniversalRenderer.sUISplitEnable != splitUIRenderScaleToggle.isOn)
        {
            SetSetting();

            Refresh();
        }
    }

    void Refresh()
    {
        if (logic)
            logic.SetData(graphicsSetting.frameRate, graphicsSetting.resolutionLevel, graphicsSetting.antialiasing);
    }

   // void OnGUI()
   // {
   //     if (graphicsSetting && graphicsSetting.displayFPS)
   //     {
   //         GUI.Label(new Rect(10, 10, 200, 200), "FPS:" + logic.f_Fps.ToString("f2"), style);
   //         //GUI.Label(new Rect(0, 20, 100, 100), "AverageFPS:" + ((int)averageFPS).ToString(), style);
   //         GUI.Label(new Rect(10, 45, 100, 100), Screen.width.ToString() + " " + Screen.height.ToString(), style);
   //        // GUI.Label(new Rect(0, 70, 100, 100), ((int)Screen.width).ToString() + " " + ((int)Screen.height).ToString(), style);
   //     }
   // }
}
