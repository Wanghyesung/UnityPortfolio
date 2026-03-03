using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public enum SkillAnimPlayMode
{
    None,           // 애니메이션 재생 안함
    Trigger,        // 트리거 하나 세팅
    Bool,           // bool 하나 세팅
    CrossFadeState  // 특정 스테이트로 강제 CrossFade
}

[CreateAssetMenu(menuName = "SO/Profiles/SkillAnimation", fileName = "AnimationProfile")]
public class SKillAnimation : ScriptableObject
{
    [Header("Trigger Mode")]
    public string triggerName;          // "Attack1_Trigger"

    [Header("Bool Mode")]
    public string boolName;             // "isRunning"

    [Header("Direct State Mode")]
    public string stateName;            // "Player_Attack1"
    public int layerIndex = 0;
    public float crossFadeTime = 0.1f;

    [Header("Play Mode")]
    public SkillAnimPlayMode playMode = SkillAnimPlayMode.None;
}
