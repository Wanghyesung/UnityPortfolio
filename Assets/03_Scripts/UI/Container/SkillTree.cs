using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTree : BaseUI, IContainer
{

    [SerializeField] private ButtonUI m_pCloseButton;
    [SerializeField] private ButtonUI m_pSeleteButton;
    [SerializeField] private Container m_pSkillContainer;

    [SerializeField] private SlotContainer m_pSlotContainer;

    [SerializeField] private eContainerType m_eContainerType = eContainerType.SkillTree; // ← 인스펙터에 드롭다운으로 보임
    public eContainerType ContainerType { get => m_eContainerType; }

    public void Init()
    {
        m_pSkillContainer.SetParent(this);
        m_pSkillContainer.Build();
    }

    public void SetVisible(bool _bOn)
    {
        m_pSkillContainer.ClearTarget();
        gameObject.SetActive(_bOn);

    }
    protected override void Awake()
    {
        base.Awake();

       
        //컨테이너에서 스킬 눌렸다면 가져올 수 있게
        m_pSkillContainer.OnSelectEvt += select_skill;
    }

    private void Update()
    {
        
    }

    private void OnDisable()
    {
        m_pSlotContainer.UnActiveSlot();
    }
    public void CloseTap()
    {
        gameObject.SetActive(false);   
    }
    private void select_skill()
    {
        SlotView pTargetView = m_pSkillContainer.GetTargetSlot();
        if(pTargetView.SOEntryUI == null) 
            return;

        //데이터 서비스에서 지금 눌린 데이터 참조
        DataService.m_Instance.StartPickData(this, pTargetView.SOEntryUI, pTargetView.SlotIdx, 1);

        //인터페이스 매니저를 만들어서 해당 클래스에게 요청하는 식으로 변경
        m_pSlotContainer.UnActiveSlot();
        m_pSlotContainer.ActiveSlot(pTargetView.SOEntryUI.GetUIHashCode());
    }

    //IContainer 구현
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) {}

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0) { return null; }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0) { return -1; }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0) { return -1; }

    public bool AddData(SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        return false;
    }

    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0) { return false; }
    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0) { return false; }
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        //기존 스킬창에서 스킬이 지워지는 일은 없음
        m_pSkillContainer.ClearTarget();
        return true;
    }
    public bool DeleteData(SOEntryUI _pSOData, int _iAmount)
    {
        return false;
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx) { return false; }


}
