using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;


//NPC 대화 목적
[Serializable]
public enum eSpeechAction
{
    Store,      //상점 열기
    Quest,      //퀘스트 받기
    End,
}

[CreateAssetMenu(menuName = "SO/NPC/Speech")]
public class SOSpeech : ScriptableObject
{
    public LocalizedString Message; //NPC 대화 내용

    public SpeechChoice Choice;     //대화 선택지
}

//대회를 이어갈지, 퀘스트를 수락할지 거절할지 선택지
[Serializable]
public class SpeechChoice
{
    public LocalizedString PositiveText = null;        // 선택지에 표시될 문장
    public LocalizedString NegativeText = null;

    public SOSpeech NextSpeech;                        // 이 선택지 선택 후 넘어갈 다음 대화(없으면 대화 종료)
    public eSpeechAction Action;                       // 긍정적 대답에 대한 행동
}