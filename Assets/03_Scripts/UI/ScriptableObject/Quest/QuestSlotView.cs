using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestSlotView : SlotView
{
    private long QuestID = -1;

    private SOSpeechInfoUI m_pSOQuest = null;
    public SOSpeechInfoUI SOQuest { get => m_pSOQuest; }

    override protected void Awake()
    {
        base.Awake();
    }

    public override void Bind(SOEntryUI _pEntryUI, int _iSlotIdx)
    {
        m_pTargetSO = _pEntryUI;
        m_iSlotIdx = _iSlotIdx;
        m_pSOQuest = _pEntryUI as SOSpeechInfoUI;
        if (m_pSOQuest == null)
        {
            m_pIcon.enabled = false;
            m_pTextMeshProUGUI.text = "";
        }
        else
        {
            m_pIcon.enabled = true;
            m_pTextMeshProUGUI.text = m_pSOQuest.SpeechTitle.GetLocalizedString();
        }
    }

}
