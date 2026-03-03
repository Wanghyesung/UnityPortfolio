using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDescInfo : BaseUI
{
    
    [SerializeField] private ButtonUI m_pCloseButton = null;
    [SerializeField] private ButtonUI m_pCompletedButton = null;
    [SerializeField] private ButtonUI m_pGiveUPButton = null;

    [SerializeField] private TextMeshProUGUI m_pTMPQuestTitle = null;
    [SerializeField] private TextMeshProUGUI m_pTMPQuestDesc = null;
    [SerializeField] private TextMeshProUGUI m_pTMPQuestProgress = null;



    private SOSpeechInfoUI m_pTargetQuest = null;
    private Image m_pComButtonImage = null; 
    private Color m_tComButtonColor = Color.white;
    protected override void Awake()
    {
        base.Awake();

        m_pGiveUPButton.OnClickEvt += giveup_quest;
        m_pCompletedButton.OnClickEvt += completed_quest;

        m_pComButtonImage = m_pCompletedButton.GetComponent<Image>();
        m_tComButtonColor = m_pComButtonImage.color;
    }

    public void SettingQuest(SOSpeechInfoUI _pTargetQuest, float _fProgress)
    {
       
        m_pTargetQuest = _pTargetQuest;
        m_pTMPQuestTitle.text = _pTargetQuest.SpeechTitle.GetLocalizedString(); 
        m_pTMPQuestDesc.text = _pTargetQuest.SpeechDescription.GetLocalizedString();

        m_pTMPQuestProgress.text = $"({_fProgress}%)";

        if (_fProgress >= 100.0f)
        {
            m_pCompletedButton.SetRaycast(true);
            m_pComButtonImage.color = m_tComButtonColor;
        }
        else
        {
            m_pCompletedButton.SetRaycast(false);
            Color tColor = m_tComButtonColor;
            tColor.r /= 2.0f;
            m_pComButtonImage.color = tColor;
        }
    }

    private void giveup_quest()
    {
        QuestManager.m_Instance.DeleteQuest(m_pTargetQuest);
        gameObject.SetActive(false);
    }
    

    private void completed_quest()
    {
        float fProgress = QuestManager.m_Instance.GetProgress(m_pTargetQuest);
        if (fProgress >= 100.0f)
        {
            QuestManager.m_Instance.CompletedQuest(m_pTargetQuest);
            CloseTap();
        }
    }

    public void CloseTap()
    {
        m_pTargetQuest = null;
        gameObject.SetActive(false);
    }
}
