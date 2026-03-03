using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[Serializable]
public class QuestReward
{
    public SOItem Item;    // 아이템 SO
    public int Amount;     // 보상 개수
}

//퀘스트 타입
[Serializable]
public enum eQuestType
{
    Kill,          
    Collect,
    Talk,
    End
}

[CreateAssetMenu(menuName = "SO/NPC/SpeechInfo")]
public class SOSpeechInfoUI : SOEntryUI
{
    public LocalizedString SpeechTitle;             // 퀘스트 제목
    public LocalizedString SpeechDescription;       // 퀘스트 설명

    public SOQuestInfo QuestInfo;                   // 퀘스트 정보
    public QuestReward QuestReward;                 // 퀘스트 보상

}
