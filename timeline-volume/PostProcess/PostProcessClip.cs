using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class PostProcessClip : PlayableAsset, ITimelineClipAsset
{
    public PostProcessBehaviour template = new PostProcessBehaviour();

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Extrapolation | ClipCaps.Blending; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<PostProcessBehaviour>.Create(graph, template);
        PostProcessBehaviour clone = playable.GetBehaviour();
        return playable;
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(PostProcessClip))]
public class PostProcessClipEditor : Editor
{
    PostProcessClip postProcessClip;
    Editor profileEditor;
    SerializedProperty profileProperty;
    SerializedProperty curveProperty;

    void OnEnable()
    {
        postProcessClip = target as PostProcessClip;
        profileEditor = Editor.CreateEditor(postProcessClip.template.profile);
        profileProperty = serializedObject.FindProperty("template.profile");
        curveProperty = serializedObject.FindProperty("template.weightCurve");
    }

    void OnDisable()
    {
        DestroyImmediate(profileEditor);
    }

    public override void OnInspectorGUI()
    {
        postProcessClip.template.layer = EditorGUILayout.LayerField("Layer", postProcessClip.template.layer);
        serializedObject.Update();
        EditorGUILayout.PropertyField(profileProperty);
        EditorGUILayout.PropertyField(curveProperty);
        serializedObject.ApplyModifiedProperties();

        profileEditor?.OnInspectorGUI();
    }
}
#endif