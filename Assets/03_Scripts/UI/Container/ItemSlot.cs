using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SOItemUI;
using static SOSkillUI;

public class ItemSlot : Slot
{
    [SerializeField] private SOItemUI m_pSOItem;

    [Header("Item Type")]
    [SerializeField] private eItemType m_eItemType = eItemType.None;

    [SerializeField] private TextMeshProUGUI m_pCountBadge = null;

    public SOItemUI SOItem { get => m_pSOItem; }

    private EffectContext m_pItemEquipContext = new EffectContext();

    protected override void Awake()
    {
        base.Awake();
        m_pCheckUI.OnClickEvt += selete_item_slot;

        m_iUIType |= (uint)m_eItemType << (int)SOEntryUI.eUIType.Item;
    }

    public override void Init()
    {
        base.Init();
        m_iUIType |= (uint)m_eItemType << (int)SOEntryUI.eUIType.Item;
    }
   

    public override void Bind(SOEntryUI _pSOTarget)
    {
       
        base.Bind(_pSOTarget);

        if (_pSOTarget != null)
        {
            m_pSOItem = _pSOTarget as SOItemUI;
            SetCoolTime(m_pSOItem.ItemData.cooldown);
            UpdateCount();
        } 
        else
        {
            m_pSOTarget = null;
            SetCoolTime(0.0f);
        }
    }

    public override void Using()
    {
        if (m_pSOItem == null)
            return;
        
        int iConsumeCount = 1;

        //데이터 사용 후 인덱스 업데이트
        if (m_pOwner?.Consume(m_iSlotIdx, iConsumeCount) == false)
        {
            Bind(null);
            return;
        }

        m_pItemEquipContext.pTarget = GameManager.m_Instance.Player.gameObject;
        ItemEffectRunner.ApplyEffectUsing(m_pSOItem.ItemData, m_pItemEquipContext);

        UpdateCount();
    }
    private void selete_item_slot()
    {
        DataService.m_Instance.TryDropDataAndSwap(m_pOwner, SlotIdx);
    }

    public void UpdateCount(int _iAmount = 0)
    {
        int iCount = 0;
        if(_iAmount > 0)
            iCount = _iAmount;
        else
            iCount = m_pOwner?.GetDataAmount(m_pSOTarget) ?? 0;

        bool bShow = iCount > 1; // 1개 이하면 보통 표기 안 함
        if (bShow)
        {
            m_pCountBadge.enabled = true;
            m_pCountBadge.text = iCount.ToString();
        }
        else
            m_pCountBadge.enabled = false;

    }

}

