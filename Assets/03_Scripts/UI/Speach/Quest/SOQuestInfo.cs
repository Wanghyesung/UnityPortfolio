using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
[CreateAssetMenu(menuName = "SO/NPC/QuestInfo")]
public class SOQuestInfo : ScriptableObject
{
    public eQuestType Type;           // 어떤 퀘스트인지 (ex: 몬스터 잡기, 아이템 모으기)
    public int QuestId;               // 퀘스트 아이디
    public int TargetId;              // 몬스터ID, 아이템ID, 지역ID
    public int TargetAmount;          // 목표 수량
}


