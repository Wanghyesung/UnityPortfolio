using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private QuestState m_pQuestState;

    private Dictionary<long, Quest> m_hashQuest =new();
    private Dictionary<long, Quest> m_hashCompletedQuest = new();

    public static QuestManager m_Instance = null;

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(this);
            return;
        }
        m_Instance = this;

        m_pQuestState.Init();
    }

    //[ 32        이벤트 타입            ][ 32           타겟 ID              ]
    public long GetHashCode(eQuestType _eType, int _iTargetID)
    {
        return (long)_eType << 32 | (long)_iTargetID;
    }

    public void AddQuest(SOSpeechInfoUI _pQuestSpeech)
    {
        long lHashCode = GetHashCode(_pQuestSpeech.QuestInfo.Type, _pQuestSpeech.QuestInfo.TargetId);
        if (m_hashCompletedQuest.ContainsKey(lHashCode) == true)
            return;

        if (m_hashQuest.ContainsKey(lHashCode) == true)
            return;

        Quest pNewQuest = new Quest(_pQuestSpeech, 0);
        m_hashQuest.Add(lHashCode, pNewQuest);
        m_pQuestState.AddQuest(_pQuestSpeech);
    }

    public void AddCompletedQuest(SOSpeechInfoUI _pQuestSpeech)
    {
        long lHashCode = GetHashCode(_pQuestSpeech.QuestInfo.Type, _pQuestSpeech.QuestInfo.TargetId);
        if (m_hashCompletedQuest.ContainsKey(lHashCode) == true)
            return;

        if(m_hashQuest.TryGetValue(lHashCode, out Quest pQuest)==true)
            m_hashCompletedQuest.Add(lHashCode, pQuest);
        
    }
    public void DeleteQuest(SOSpeechInfoUI _pQuestSpeech)
    {
        long lHashCode = GetHashCode(_pQuestSpeech.QuestInfo.Type, _pQuestSpeech.QuestInfo.TargetId);
        if (m_hashQuest.ContainsKey(lHashCode) == false)
            return;

        m_hashQuest.Remove(lHashCode);
        m_pQuestState.DeleteQuest();
    }

    public void CompletedQuest(SOSpeechInfoUI _pQuestSpeech)
    {
        QuestReward pReward = _pQuestSpeech.QuestReward;

        //아이템 매니저에서 보상 아이템 ID를 통해 아이템 데이터 가져오기
        SOEntryUI pItemData = ItemDataManager.m_Instance.GetItemData(pReward.Item.ItemID);
        DataService.m_Instance.TryAddData(eContainerType.Inventory, pItemData, pReward.Amount);

      
        //수집 퀘스트라면 인벤토리에 데이터 수량만큼 지워주기
        if(_pQuestSpeech.QuestInfo.Type == eQuestType.Collect)
        {
            //수집해야할 아이템 데이터 가져오기
            SOEntryUI pCollectItemData = ItemDataManager.m_Instance.GetItemData(_pQuestSpeech.QuestInfo.TargetId);
            DataService.m_Instance.DeleteData(eContainerType.Inventory, pCollectItemData, _pQuestSpeech.QuestInfo.TargetAmount);
        }

        //완료 퀘스트로 등록하고 퀘스트 창에서 제거
        AddCompletedQuest(_pQuestSpeech);
        DeleteQuest(_pQuestSpeech);
    }

    public void UpdateQuest(eQuestType _eType, int _iTargetID, int _iAmount)
    {
        long lHashCode = GetHashCode(_eType , _iTargetID);

        if(m_hashQuest.TryGetValue(lHashCode, out Quest pQuest) == true)
        {
            pQuest.UpdateProgress(_iAmount);
        }
    }

    public float GetProgress(SOSpeechInfoUI _pQuestSpeech)
    {
        long lHashCode = GetHashCode(_pQuestSpeech.QuestInfo.Type, _pQuestSpeech.QuestInfo.TargetId);
        if (m_hashQuest.TryGetValue(lHashCode, out Quest pQuest) == true)
            return pQuest.GetProgress();

        return 0.0f;
    }
    
    public Quest FindCompletedQuest(SOSpeechInfoUI _pQuestSpeech)
    {
        long lHashCode = GetHashCode(_pQuestSpeech.QuestInfo.Type, _pQuestSpeech.QuestInfo.TargetId);
        if (m_hashCompletedQuest.TryGetValue(lHashCode, out Quest pQuest) == true)
            return pQuest;
        return null; 
    }

    public Quest FindQuest(SOSpeechInfoUI _pQuestSpeech)
    {
        long lHashCode = GetHashCode(_pQuestSpeech.QuestInfo.Type, _pQuestSpeech.QuestInfo.TargetId);
        if (m_hashQuest.TryGetValue(lHashCode, out Quest pQuest) == true)
            return pQuest;
        return null;
    }

    public Quest FindCompletedQuest(long _lHashCode)
    {
        if (m_hashCompletedQuest.TryGetValue(_lHashCode, out Quest pQuest) == true)
            return pQuest;
        return null;
    }

    public Quest FindQuest(long _lHashCode)
    {
        if (m_hashQuest.TryGetValue(_lHashCode, out Quest pQuest) == true)
            return pQuest;
        return null;
    }

    public Quest FindQuestAll(SOSpeechInfoUI _pQuestSpeech)
    {
        //퀘스트가 아니라면
        if(_pQuestSpeech.QuestInfo == null)
            return null;

        //완료한 퀘스트 인지
        long lHashCode = GetHashCode(_pQuestSpeech.QuestInfo.Type, _pQuestSpeech.QuestInfo.TargetId);
        Quest pQuest = FindCompletedQuest(lHashCode);
        if (pQuest != null)
            return pQuest;

        pQuest = FindQuest(lHashCode);
        if (pQuest != null)
            return pQuest;

        return null;
    }


    public void SetActive(bool _bOn)
    {
        m_pQuestState.gameObject.SetActive(_bOn);
    }

}
