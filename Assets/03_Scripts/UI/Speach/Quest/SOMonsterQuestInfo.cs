using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "SO/NPC/MonsterQuestInfo")]
public class SOMonsterQuestInfo : SOQuestInfo
{
    public SOMonsterInfo SOMonster;
    private void OnValidate()
    {
        TargetId = SOMonster.MonsterID;
    }
}