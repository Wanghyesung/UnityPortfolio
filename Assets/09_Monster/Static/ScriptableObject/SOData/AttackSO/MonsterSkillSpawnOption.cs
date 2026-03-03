using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Monster/SkillSpawnOption")]

public class MonsterSkillSpawnOption : ScriptableObject
{
    [Header("Spawn Options")]
    public ePointType SpawnPoint;
    public bool SpawnPlayerPos = false;
    public bool SpawnCurPos = false;
    public bool SpawnRandomPos = false;
    public bool PlayerDir = false;
    public float SpawnDiff = 1.0f;
    public float LifeTime = 0.0f;


    //처음에 생성될때만 설정 
    [Header("Spawn Position Options")]
    public Vector3 AttackDir = Vector3.zero;
    public Vector3 SpawnPos = Vector3.zero;
    public Vector3 SpawnRot = Vector3.zero;

    public SOPoolEntry SOPoolEntry = null;

    public MonsterSkillInfo EndFrameSpawnSkill = null;
}
