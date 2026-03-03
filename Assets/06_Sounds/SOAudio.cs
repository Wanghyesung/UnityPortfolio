using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio")]
public class SOAudio : ScriptableObject
{
    public List<AudioClip> Clips = new List<AudioClip>(); //같은 오브젝트에 관련된 비슷한 음악들 (ex 불소리)
    //사운드를 어디 채널로 보낼지
    public AudioMixerGroup OutputGroup;

    [Range(0f, 1f)] public float Volume = 1.0f;
    //같은 효과음을 반복 재생하면 너무 기계적으로 들림
    [Range(0.1f, 3f)] public float PitchMin = 1.0f;
    [Range(0.1f, 3f)] public float PitchMax = 1.0f;

    public bool spatial3D = false;
    [Range(0f, 1f)] public float SpatialBlend = 1.0f; // 3D일 때 1
}
