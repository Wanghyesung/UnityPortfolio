using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SpeechTyper : ButtonUI
{
    [SerializeField] private TMP_Text m_pText;

    [SerializeField] private TMP_Text m_pPositiveText = null;
    [SerializeField] private ButtonUI m_pPositiveButton = null;

    [SerializeField] private TMP_Text m_pNagativeText = null;
    [SerializeField] private ButtonUI m_pNagativeButton = null;

    [SerializeField] private float charsPerSecond = 20f;

    private SONPCSpeech m_pNPCSpeech = null;
    private SOSpeech m_pCurSpeech = null;
   
    private float m_fCurTime = 0.0f;

    private string m_strFullString = "";
    private int m_iToalCount = -1;
    private int m_iVisibleCount = -1;

    private float m_fInterval = 0.0f;
    private Coroutine m_pTypingCoroutine;

    public void Init()
    {
        OnClickEvt += NextText;


        m_pPositiveButton.OnClickEvt += PositiveAction;
        m_pPositiveButton.gameObject.SetActive(false);

        m_pNagativeButton.OnClickEvt += CloseSpeech;
        m_pNagativeButton.gameObject.SetActive(false);
    }

    public void ShowNPCSpeech(SONPCSpeech _pNPCSpeech)
    {
        gameObject.SetActive(true);
        m_pNPCSpeech = _pNPCSpeech;
        StartSpeech(m_pNPCSpeech.Speech);
    }
    private void StartSpeech(SOSpeech _pRootSpeech)
    {
        m_pCurSpeech = _pRootSpeech;
        ShowText(m_pCurSpeech.Message.GetLocalizedString());
    }

    private void ShowText(string _strMessage)
    {
        m_fInterval = 1.0f / charsPerSecond;

        m_strFullString = _strMessage;

        m_pText.text = m_strFullString;
        
        m_pText.ForceMeshUpdate();
        m_pText.maxVisibleCharacters = 0;

        // 이전 타이핑 중이면 정지
        if (m_pTypingCoroutine != null)
            StopCoroutine(m_pTypingCoroutine);

        // 새로 시작
        m_pTypingCoroutine = StartCoroutine(TypeRoutine());
    }

    private IEnumerator TypeRoutine()
    {
        m_iToalCount = m_pText.textInfo.characterCount;
        m_iVisibleCount = 0;

        //일정한 시간동안 대화 내용을 천천히 보여주기
        while (m_iVisibleCount < m_iToalCount)
        {
            m_fCurTime += Time.deltaTime;
            while (m_fCurTime >= m_fInterval && m_iVisibleCount < m_iToalCount)
            {
                m_fCurTime -= m_fInterval;
                m_iVisibleCount++;
                m_pText.maxVisibleCharacters = m_iVisibleCount;
            }
            yield return null;
        }

        m_pTypingCoroutine = null;
    }


    private void NextText()
    {
        if (m_pTypingCoroutine != null)
            StopCoroutine(m_pTypingCoroutine);

        //다음칸으로 넘기기
        if (m_iVisibleCount == m_iToalCount)
        {
            m_pText.maxVisibleCharacters = m_iToalCount;
            Completed();
        }
        else
        {
            m_iVisibleCount = m_iToalCount;
            m_pText.maxVisibleCharacters = m_iVisibleCount;
        }
        
    }
   
    private void Completed()
    {
        string strPositive = "";
        string strNagative = "";

        ////긍정, 부정 대답이 없다면 다음 대화로 넘어가기
        if (m_pCurSpeech.Choice.PositiveText.IsEmpty == true && m_pCurSpeech.Choice.NegativeText.IsEmpty == true)
        {
            if (string.IsNullOrEmpty(strPositive) != false && string.IsNullOrEmpty(strNagative) != false)
                StartSpeech(m_pCurSpeech.Choice.NextSpeech);
        
            return;
        }

        //긍정 텍스트를 누르면 어떤 행동(상점 열기, 퀘스트 받기)을 하고 부정이면 창 닫는용도
        if(m_pCurSpeech.Choice.PositiveText.IsEmpty == false)
        {
            m_pPositiveButton.gameObject.SetActive(true);
            strPositive = m_pCurSpeech.Choice.PositiveText.GetLocalizedString();
            m_pPositiveText.text = strPositive;
        }
        if (m_pCurSpeech.Choice.NegativeText.IsEmpty == false)
        {
            m_pNagativeButton.gameObject.SetActive(true);
            strNagative = m_pCurSpeech.Choice.NegativeText.GetLocalizedString();
            m_pNagativeText.text = strNagative;
        }

    }


    private void PositiveAction()
    {
        //지금은 분기가 적으니깐 switch -> 나중에는 함수 포인터로
        switch(m_pCurSpeech.Choice.Action)
        {
            case eSpeechAction.Store:
                {
                    DataService.m_Instance.SetVisibleContainer(eContainerType.Store, true);
                }
                break;
            case eSpeechAction.Quest:
                {
                    QuestManager.m_Instance.AddQuest(m_pNPCSpeech.SpeechInfo);
                }
                break;
        }

        if (m_pCurSpeech.Choice != null && m_pCurSpeech.Choice.NextSpeech != null)
            StartSpeech(m_pCurSpeech.Choice.NextSpeech);
        else
            CloseSpeech();
    }

    private void CloseSpeech()
    {
        //리빌딩 예약
        m_pNagativeButton.gameObject.SetActive(false);
        m_pPositiveButton.gameObject.SetActive(false);

        SpeechManager.m_Instance.CloseSpeechdUI();
    }


}
