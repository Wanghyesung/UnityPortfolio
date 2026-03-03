using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//퀘스트는 기본적으로 하나만 사용 다른 NPC마다 quest를 가지고 있는건 메모리 낭비
public class QuestUI : ButtonUI
{
    private Action m_pClickEvent = null; 
  
    protected override void Awake()
    {
        base.Awake();
        OnClickEvt += ShowText;
    }
    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    //눌렀을 때 발동
    public void ShowText()
    {
        //SpeechManager.m_Instance.ClickQuestUI();
        m_pClickEvent?.Invoke();
        SpeechManager.m_Instance.CloseWorldUI();
    }

    public void SetClickEvent(Action _pEvent)
    {
        m_pClickEvent = _pEvent; 
    }


}
