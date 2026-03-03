using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestState : BaseUI
{
    [SerializeField] private Container m_pQuestContainer = null;

    [SerializeField] private QuestDescInfo m_pQuestDescInfo;

    //questmanaer랑 분리하기, questbar에 container넣어서 눌리면 questmanaer에 전달,
    //questState는 굳이 데이터 쪽이랑 붙이지말기
    
    protected override void Awake()
    {
        base.Awake();


        //컨테이너에서 스킬 눌렸다면 가져올 수 있게
        m_pQuestContainer.OnSelectEvt += select_quest;
    }

    public void Init()
    {
        m_pQuestContainer.Build();
    }

    public void Close()
    {
        m_pQuestDescInfo.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void select_quest()
    {
        SlotView pTargetSlot = m_pQuestContainer.GetTargetSlot();
        SOSpeechInfoUI pQuestUI = pTargetSlot.SOEntryUI as SOSpeechInfoUI;
        if (pQuestUI == null)
            return;

        m_pQuestDescInfo.gameObject.SetActive(true);

       
        float fProgress = QuestManager.m_Instance.GetProgress(pQuestUI);
        m_pQuestDescInfo.SettingQuest(pQuestUI, fProgress);
    }

    public void CloseTap()
    {
        m_pQuestDescInfo.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void AddQuest(SOSpeechInfoUI _pSpeechInfo)
    {
        m_pQuestContainer.AddData(_pSpeechInfo);
    }

    public void DeleteQuest()
    {
        int _iIdx = m_pQuestContainer.GetTargetSlot().SlotIdx;
        m_pQuestContainer.DeleteData(_iIdx);
        m_pQuestContainer.SortData();
    }

    public void UpdateProgress(float _fProgress)
    {
        //m_pQuestDescInfo.
    }

   
}
