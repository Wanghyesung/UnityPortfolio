using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Profiles/Targeting", fileName = "TargetingProfile")]
public class TargetingProfile : ScriptableObject
{
    [Min(0f)] public int MaxTargets = 1;        // 최대 대상 수 (0 = 무제한)
    public LayerMask TargetLayers;              // 대상 레이어
    public Game.Common.TargetType targetType;   // 대상 유형 (자신/적군/지역/모두)


    public bool targetPosition = false;       //타겟 위치로
    public bool playerFacing = false;         // 플레이어가 바라보는 방향으로 스킬 발동
    public Vector3 offset = Vector3.zero;     // 생성될 위치 오프셋
    public Vector3 offsetRot = Vector3.zero;  // 생성될 회전 오프셋
    public float spawnDistance = 0f;          // 스킬이 생성될 거리
}

