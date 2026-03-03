using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPoolAble
{
    [SerializeField] private SOItem m_pItem;
    public SOItem GetItem() => m_pItem;

    private int iPlayerLayer = -1;

    private float m_fCurTime = 0.0f;
    private float m_fLifeTime = 10.0f;

    private int m_iPushPoolCount = 1;
    [SerializeField] private bool m_bDontDestroy = false;

    [SerializeField] private SOAudio m_pGetItemAudio = null;
    public void OnSpawn()
    {
        m_fCurTime = 0.0f;
        m_iPushPoolCount = 1;
    }
    public void OnDespawn()
    {

    }

    private void Update()
    {
        if (m_bDontDestroy == true)
            return;

        m_fCurTime += Time.deltaTime;
        if (m_fCurTime > m_fLifeTime)
            PushObjectPool();

    }

    public int GetItemID()
    {
        return m_pItem.ItemID;
    }


    private void OnTriggerEnter(Collider other)
    {
        //플레이어만 아이템을 먹을 수 있게
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //아이템 매니저에서 내 아이템 ID를 바탕으로 아이템 데이터 가져오기
            SOEntryUI pItemData = ItemDataManager.m_Instance.GetItemData(m_pItem.ItemID);
            if (DataService.m_Instance.TryAddData(eContainerType.Inventory, pItemData, 1) == true)
                PushObjectPool();

            //아이템 줍기 퀘스트 업데이트
            QuestManager.m_Instance.UpdateQuest(eQuestType.Collect, m_pItem.ItemID, 1);

            if (m_pGetItemAudio != null)
                SoundManager.m_Instance.PlaySfx(m_pGetItemAudio, null);
        }
    }

    public void PushObjectPool()
    {
        if (m_iPushPoolCount <= 0)
            return;

        --m_iPushPoolCount;
        ObjectPoolManager.m_Instance.PushObject(ePoolType.Global, m_pItem.ItemObject.AssetGUID, gameObject);
    }
}
