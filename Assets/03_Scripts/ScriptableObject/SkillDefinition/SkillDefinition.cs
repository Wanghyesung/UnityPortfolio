using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Game.Common;


[CreateAssetMenu(fileName = "SkillDefinition", menuName = "SO/Skill", order = int.MaxValue)]
public class SOSKill : ScriptableObject
{
    [Header("Meta")]
    [SerializeField]        private MetaProfile meta;               //스킬 정보
    [Header("Damage")]
    [SerializeField]        private DamageProfile damage;           //공격 계수
    [Header("Defense")]
    [SerializeField]        private DefenseProfile defense;         //방어 계수
    [Header("Buff")]
    [SerializeField]        private BuffProfile buff;               //버프 계수
    [Header("Option")]
    [SerializeField]        private SkillOptionProfile option;      //스킬 옵션
    [Header("Logic")]
    [SerializeField]        private LogicProfile logic;             //스킬 로직
    [Header("Animation")]
    [SerializeField]        private SKillAnimation animation;       //스킬 애니메이션

    public MetaProfile Meta => meta;
    public DamageProfile Damage => damage;
    public DefenseProfile Defense => defense;
    public BuffProfile Buff => buff;
    public SkillOptionProfile Option => option;
    public LogicProfile Loggic => logic;
    public SKillAnimation Animation => animation;
}
