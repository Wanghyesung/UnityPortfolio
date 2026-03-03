using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestChoice : MonoBehaviour
{
    [SerializeField] private Container m_pConatiner;
    private List<SONPCSpeech> m_listNPCSpeech = null;
    private void Awake()
    {
        m_pConatiner.Build();
        m_pConatiner.OnSelectEvt += speech;
    }

    private void speech()
    {
        //퀘스트 선택창에서 누른 퀘스트 가져오기
        SlotView pSlot = m_pConatiner.GetTargetSlot();
        
        if(pSlot == null)
            gameObject.SetActive(false);
        else
        {
            //해당 퀘스트 대화 시작
            SOSpeechInfoUI pSpeechUI = pSlot.SOEntryUI as SOSpeechInfoUI;
            if (pSpeechUI == null)
                return;

            SONPCSpeech pSpeech = FindNCPSpeech(pSpeechUI.QuestInfo.QuestId);
            SpeechManager.m_Instance.ShowText(pSpeech);
        }

        m_listNPCSpeech = null;
        gameObject.SetActive(false);
    }

    public void ShowQuest(List<SONPCSpeech> _listNPCSPeech)
    {
        //퀘스트 선택창 열기
        gameObject.SetActive(true);

        //기존 데이터 지우고 새로운 데이터로
        m_listNPCSpeech = _listNPCSPeech;

        //기존 컨테이너의 참조 데이터 해제
        m_pConatiner.ClearData();
    
        for(int i = 0; i< m_listNPCSpeech.Count; ++i)
        {
            //이미 클리어한 퀘스트인지 확인 후 컨테이너에 데이터 넣기
            if(QuestManager.m_Instance.FindQuestAll(m_listNPCSpeech[i].SpeechInfo) == null)
                m_pConatiner.AddData(m_listNPCSpeech[i].SpeechInfo);
        }
        
    }

    public SONPCSpeech FindNCPSpeech(int _iQuestID)
    {
        for(int i = 0; i<m_listNPCSpeech.Count; ++i)
        {
            if (m_listNPCSpeech[i].SpeechInfo.QuestInfo.QuestId == _iQuestID)
                return m_listNPCSpeech[i];
        }
        return null;
    }

}
