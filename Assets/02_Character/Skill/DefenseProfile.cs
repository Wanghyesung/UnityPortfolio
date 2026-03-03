using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Profiles/Defense", fileName = "DefenseProfile")]

public class DefenseProfile : ScriptableObject
{
    [Header("Base")]
    public float baseDefense;              // 기본 방어
    public int   defenseCount;             // 방어 횟수
}
