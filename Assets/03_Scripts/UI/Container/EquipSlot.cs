using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EquipSlot : Slot
{
    [SerializeField] private SOEquipUI m_pEquip = null;
    [SerializeField] private SOEquipUI m_pPreEquip = null;
    public SOEquipUI SOEquip { get => m_pEquip; }

    [SerializeField] private eEquipType m_eEquipType = eEquipType.None;

    private GameObject m_pEquipObject = null;

    //아이템이 컨텍스트를 들고있으면 해당 아이템을 들고있는 오브젝트를 슬롯이 들고있거나, 
    //아이템 컨텍스트를 슬롯에서 들고있어야한다. 아니면 사용할 때마다 가져오기..
    //아이템은 월드 오브젝트로 먹으면 풀에 다시 넣어야하는데 내가 여기서도 참조하고 있으면 꼬드가 굉장히 더러워질 수 있음

    //아이템은 Value로만 계수를 들고있고 bind될 때 context에서 해당 값들 복사
    private EffectContext m_pItemEquipContext = new EffectContext();

    protected override void Awake()
    {
        base.Awake();
        m_pCheckUI.OnClickEvt += selete_equip_slot;

        m_iUIType |= (uint)m_eEquipType << (int)SOEntryUI.eUIType.Equip;
    }

    public override void Init()
    {
        base.Init();
        m_iUIType |= (uint)m_eEquipType << (int)SOEntryUI.eUIType.Equip;
    }
    public override void Bind(SOEntryUI _pSOTarget)
    {
        base.Bind(_pSOTarget);

        m_pPreEquip = m_pEquip;

        if (_pSOTarget != null)
            m_pEquip = _pSOTarget as SOEquipUI;
        else
            m_pSOTarget = null;

        equiped();
    }

    public override void Using()
    {
        if (m_pEquip != null)
        {
            m_pItemEquipContext.pTarget = GameManager.m_Instance.Player.gameObject;
            ItemEffectRunner.ApplyEffectUsing(m_pEquip.ItemData, m_pItemEquipContext);
        }

    }

    private void equiped()
    {
        //이미 등록된 아이템이 있다면 아이템 효과 해제
        if (m_pEquipObject != null)
        {
            ItemEffectRunner.ApplyEffectRelease(m_pPreEquip.ItemData, m_pItemEquipContext);

            ObjectPoolManager.m_Instance.PushObject(ePoolType.Global, m_pPreEquip.ItemData.ItemObject.AssetGUID, m_pEquipObject);
        }

        if (m_pEquip.ItemData == null)
            return;

        //해당 아이템 오브젝트 가져오기
        GameObject pEquip =
            ObjectPoolManager.m_Instance.GetObject(ePoolType.Global, m_pEquip.ItemData.ItemObject.AssetGUID, Vector3.zero, Vector3.zero);

        if (pEquip == null)
            return;

        //현재 내 아이템 설정 후 아이템 효과 적용
        m_pEquipObject = pEquip;
        m_pItemEquipContext.pTarget = GameManager.m_Instance.Player.gameObject;
        m_pItemEquipContext.pOwner = m_pEquipObject;

        //아이템 기본 스탯(정적 데이터)에 내 추가 아이템 효과Context(동적 데이터)를 적용
        ItemEffectRunner.ApplyEffectEquipped(m_pEquip.ItemData, m_pItemEquipContext);
    }

    private void selete_equip_slot()
    {
        DataService.m_Instance.TryDropDataAndSwap(m_pOwner, SlotIdx);
    }


  
}
