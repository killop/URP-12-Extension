#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif
using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal
{
    [Serializable]
    public class PostProcessData : ScriptableObject
    {
#if UNITY_EDITOR
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812")]
        internal class CreatePostProcessDataAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var instance = CreateInstance<PostProcessData>();
                AssetDatabase.CreateAsset(instance, pathName);
                ResourceReloader.ReloadAllNullIn(instance, UniversalRenderPipelineAsset.packagePath);
                Selection.activeObject = instance;
            }
        }

        [MenuItem("Assets/Create/Rendering/URP Post-process Data", priority = CoreUtils.Sections.section5 + CoreUtils.Priorities.assetsCreateRenderingMenuPriority)]
        static void CreatePostProcessData()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, CreateInstance<CreatePostProcessDataAsset>(), "CustomPostProcessData.asset", null, null);
        }

        internal static PostProcessData GetDefaultPostProcessData()
        {
            var path = System.IO.Path.Combine(UniversalRenderPipelineAsset.packagePath, "Runtime/Data/PostProcessData.asset");
            return AssetDatabase.LoadAssetAtPath<PostProcessData>(path);
        }
       
#endif

        [Serializable, ReloadGroup]
        public sealed class ShaderResources
        {
            [Reload("Shaders/PostProcessing/StopNaN.shader")]
            public Shader stopNanPS;

            [Reload("Shaders/PostProcessing/SubpixelMorphologicalAntialiasing.shader")]
            public Shader subpixelMorphologicalAntialiasingPS;

            [Reload("Shaders/PostProcessing/GaussianDepthOfField.shader")]
            public Shader gaussianDepthOfFieldPS;

            [Reload("Shaders/PostProcessing/BokehDepthOfField.shader")]
            public Shader bokehDepthOfFieldPS;

            [Reload("Shaders/PostProcessing/CameraMotionBlur.shader")]
            public Shader cameraMotionBlurPS;

            [Reload("Shaders/PostProcessing/PaniniProjection.shader")]
            public Shader paniniProjectionPS;

            [Reload("Shaders/PostProcessing/LutBuilderLdr.shader")]
            public Shader lutBuilderLdrPS;

            [Reload("Shaders/PostProcessing/LutBuilderHdr.shader")]
            public Shader lutBuilderHdrPS;

            [Reload("Shaders/PostProcessing/Bloom.shader")]
            public Shader bloomPS;

            [Reload("Shaders/PostProcessing/LensFlareDataDriven.shader")]
            public Shader LensFlareDataDrivenPS;

            [Reload("Shaders/PostProcessing/ScalingSetup.shader")]
            public Shader scalingSetupPS;

            [Reload("Shaders/PostProcessing/EdgeAdaptiveSpatialUpsampling.shader")]
            public Shader easuPS;

            [Reload("Shaders/PostProcessing/UberPost.shader")]
            public Shader uberPostPS;

            [Reload("Shaders/PostProcessing/FinalPost.shader")]
            public Shader finalPostPassPS;
        }

        [Serializable, ReloadGroup]
        public sealed class TextureResources
        {
            // Pre-baked noise
            [Reload("Textures/BlueNoise16/L/LDR_LLL1_{0}.png", 0, 32)]
            public Texture2D[] blueNoise16LTex;

            // Post-processing
            [Reload(new[]
            {
                "Textures/FilmGrain/Thin01.png",
                "Textures/FilmGrain/Thin02.png",
                "Textures/FilmGrain/Medium01.png",
                "Textures/FilmGrain/Medium02.png",
                "Textures/FilmGrain/Medium03.png",
                "Textures/FilmGrain/Medium04.png",
                "Textures/FilmGrain/Medium05.png",
                "Textures/FilmGrain/Medium06.png",
                "Textures/FilmGrain/Large01.png",
                "Textures/FilmGrain/Large02.png"
            })]
            public Texture2D[] filmGrainTex;

            [Reload("Textures/SMAA/AreaTex.tga")]
            public Texture2D smaaAreaTex;

            [Reload("Textures/SMAA/SearchTex.tga")]
            public Texture2D smaaSearchTex;
        }

        public ShaderResources shaders;
        public TextureResources textures;
        public Shader[] xPostProcessShaders;
#if UNITY_EDITOR
        public void ReloadXPostProcessShaders()
        {
            var guids = AssetDatabase.FindAssets("t:Shader");
            var targetDirectory = UniversalRenderPipelineAsset.packagePath + "/Runtime/Overrides/X-PostProcessing-URP/Volume";
            List<Shader> shaders = new List<Shader>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Replace('\\','/').StartsWith(targetDirectory))
                {
                    var shader = AssetDatabase.LoadAssetAtPath<Shader>(path) as Shader;
                    shaders.Add(shader);
                }
            }
            this.xPostProcessShaders = shaders.ToArray();
        }

        [MenuItem("Tools/ReloadDefaultPostProcessData", priority = CoreUtils.Sections.section5 + CoreUtils.Priorities.assetsCreateRenderingMenuPriority)]
        static void ReloadDefaultPostProcessData()
        {
            var ppd = GetDefaultPostProcessData();
            var root = UniversalRenderPipelineAsset.packagePath;
            ResourceReloader.ReloadAllNullIn(ppd, root);
            ppd.ReloadXPostProcessShaders();
            EditorUtility.SetDirty(ppd);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
