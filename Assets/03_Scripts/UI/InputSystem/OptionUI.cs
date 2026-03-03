using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionUI : MonoBehaviour
{

    [SerializeField] private ButtonUI m_pSkillButton = null;
    [SerializeField] private ButtonUI m_pInfoButton = null;
    [SerializeField] private ButtonUI m_pQuestButton = null;
    [SerializeField] private ButtonUI m_pSystemOptionButton = null;

    [SerializeField] private ButtonUI m_pOptionButton = null;
    [SerializeField] private GameObject m_pContent = null;
    private void Awake()
    {
        m_pSkillButton.OnClickEvt += visible_skill;
        m_pInfoButton.OnClickEvt += visible_infop;
        m_pQuestButton.OnClickEvt += visivle_quest;
        m_pSystemOptionButton.OnClickEvt += visivle_system;


        m_pSystemOptionButton.OnClickEvt += close_content;
        m_pOptionButton.OnClickEvt += close_content;
        m_pSkillButton.OnClickEvt += close_content;
        m_pInfoButton.OnClickEvt += close_content;
        m_pQuestButton.OnClickEvt += close_content; 
    }

    
    private void visible_skill()
    {
        DataService.m_Instance.SetVisibleContainer(eContainerType.SkillTree, true);

    }
    private void visible_infop()
    {
        DataService.m_Instance.SetVisiblePlayerInfo(true);
    }

    private void visivle_quest()
    {
        QuestManager.m_Instance.SetActive(true);
    }

    private void visivle_system()
    {
        SystemController.m_Instance.VisivleOption();   
    }

    private void close_content()
    {
        m_pContent.SetActive(!m_pContent.activeSelf);
    }
}
