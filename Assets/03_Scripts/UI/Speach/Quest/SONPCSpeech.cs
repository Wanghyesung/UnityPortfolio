using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(menuName = "SO/NPC/NPCSpeech")]
public class SONPCSpeech : ScriptableObject
{
    public SOSpeech Speech;             //대화 내용
    public SOSpeechInfoUI SpeechInfo;   //퀘스트 내용
}
