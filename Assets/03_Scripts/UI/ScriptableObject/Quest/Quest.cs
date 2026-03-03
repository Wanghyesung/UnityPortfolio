using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Quest 
{

    public SOSpeechInfoUI m_pSOQuestSpeech;
    public int m_iCurrentAmount = 0;
    public int m_iAmount = 0;


    public Quest(SOSpeechInfoUI _pQuest, int _iCurrentAmount)
    {
        m_pSOQuestSpeech = _pQuest;
        m_iAmount = _pQuest.QuestInfo.TargetAmount;
        m_iCurrentAmount = _iCurrentAmount;
    }


    public float GetProgress()
    {
        float fProgress = ((float)m_iCurrentAmount / m_iAmount) * 100.0f;
        if (fProgress > 100.0f)
            fProgress = 100.0f;

        return fProgress;
    }

    public bool UpdateProgress(int _iAmount)
    {
        m_iCurrentAmount += _iAmount;
        if(m_iCurrentAmount >= m_iAmount)
        {
            //보상 UI 활성화
            return true;
        }
        
        return false;
    }

}
