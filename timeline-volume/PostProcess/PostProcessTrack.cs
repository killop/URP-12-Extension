using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.98f, 0.27f, 0.42f)]
[TrackClipType(typeof(PostProcessClip))]
public class PostProcessTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var scriptPlayable = ScriptPlayable<PostProcessMixerBehaviour>.Create(graph, inputCount);
        return scriptPlayable;
    }
}

