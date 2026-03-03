using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Profiles/Damage", fileName = "DamageProfile")]
public class DamageProfile : ScriptableObject
{
    [Header("Base")]
    public float baseDamage;               // 기본 데미지
    public float coefficient;              // 계수
    public float criticalChance;           // 치명타 확률
    public float criticalMultiplier;       // 치명타 배율
    public float damageVariance;           // 데미지 변동폭

    public float power = 1.0f;                         //스킬 파워
    public float startAttackTime = 0.0f;               //공격 시각
    public float moveSpeed = 0.0f;                     //이동 속도
}