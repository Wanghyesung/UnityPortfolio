using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Monster/SkillOption")]

public class MonsterSkillOption : ScriptableObject
{
    public LayerMask TargetLayers;

    public float    CoolTime = 0.0f;
    public float    AttackRange = 0.0f;
    public float    AttackDamage = 0.0f;
    public float    AttackPower = 0.0f;
    public float    AttackVariance = 0.0f;
    public float    AttackTime = 0.0f;
    public float    MoveSpeed = 0.0f;
    public bool     isDown = false;
    public bool     DestroyOn = false;
}
