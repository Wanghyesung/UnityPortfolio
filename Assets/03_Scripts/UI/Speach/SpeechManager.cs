using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechManager : MonoBehaviour
{
    [SerializeField] private SpeechTyper m_pSpeechTyper = null; //npc text입력기
    [SerializeField] private QuestChoice m_pQuestChoice = null;//npc quest선택 창

    private SONPCSpeech m_pNPCSpeech = null;
    private List<SONPCSpeech> m_listNPCSpeech = null;
    [SerializeField] private QuestUI m_pWorldUI = null;
    [SerializeField] private Canvas m_pSpeechCanvas = null;

    static public SpeechManager m_Instance;

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;
        m_pSpeechTyper.Init();
    }

   
    public void ShowText()
    {
        //NPC 앞으로 카메라 자연스럽게 이동
        CameraManager.m_Instance.SmoothMove();

        //NPC 대화창 열기
        m_pSpeechCanvas.gameObject.SetActive(true);
        m_pSpeechTyper.ShowNPCSpeech(m_pNPCSpeech);
    }
    public void ShowText(SONPCSpeech _pNPCSPeech)
    {
        m_pNPCSpeech = _pNPCSPeech;
        ShowText();
        
    }

    public void ShowQuest()
    {
        m_pSpeechCanvas.gameObject.SetActive(true);
        m_pQuestChoice.ShowQuest(m_listNPCSpeech);
    }

    public void ShowSpeechUI(in Vector3 _vTargetPos , SONPCSpeech _pNPCSpeeCh)
    {
        ShowWorldUI(in _vTargetPos);

        m_pNPCSpeech = _pNPCSpeeCh;
        m_pWorldUI.SetClickEvent(ShowText);

    }
    public void ShowQuestUI(in Vector3 _vTargetPos, List<SONPCSpeech> _listNPCSpeech)
    {
        ShowWorldUI(in _vTargetPos);

        m_listNPCSpeech = _listNPCSpeech;
        m_pWorldUI.SetClickEvent(ShowQuest);
    }

    private void ShowWorldUI(in Vector3 _vTargetPos)
    {
        m_pWorldUI.gameObject.SetActive(true);
        m_pWorldUI.transform.position = _vTargetPos;
    }
    public void CloseSpeechdUI()
    {
        CloseWorldUI();
        m_pSpeechTyper.gameObject.SetActive(false);
        m_pSpeechCanvas.gameObject.SetActive(false);
       
        m_pNPCSpeech = null;
        m_listNPCSpeech = null;

        //메인카메라로 전환
        CameraManager.m_Instance.MainCameraMove();
        GameManager.m_Instance.UnLockPlayer();
    }

    public void CloseWorldUI()
    {
        m_pWorldUI.gameObject.SetActive(false);
    }

   
}
