using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInterfaceSlot : BaseUI, IContainer
{
    [SerializeField] private SlotContainer m_pSlotContainer = null;
    Dictionary<int, int> m_hashItemCount = new Dictionary<int, int>();

    [SerializeField] private eContainerType m_eContainerType = eContainerType.Interface; // ← 인스펙터에 드롭다운으로 보임
    public eContainerType ContainerType { get => m_eContainerType; }

    public static int SLOT_SIZE = 5;

    public void OnValidate()
    {
        SLOT_SIZE = m_pSlotContainer.SlotList.Count;
    }
    protected override void Awake()
    {
        base.Awake();

        m_pSlotContainer.Init();
    }

    //IContainer 구현
    public void Init()
    {
        //m_pSlotContainer.Init();
    }

    public void SetVisible(bool _bOn)
    {
        gameObject.SetActive(_bOn);
    }
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        return m_pSlotContainer.GetData(_iDataIdx, _iCategoryIdx);
    }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pData = GetData(_iDataIdx);
        if (pData == null)
            return 0;

        int iAmount = 1;
        if (m_hashItemCount.TryGetValue(pData.Id, out iAmount) == false)
            return 0;

        return iAmount;
    }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0)
    {
        if (_pSoData == null)
            return 0;

        int iAmount = 0;
        if (m_hashItemCount.TryGetValue(_pSoData.Id, out iAmount) == false)
            return 0;

        return iAmount;
    }

    public bool AddData(SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        if (m_hashItemCount.TryGetValue(_pSOData.Id, out int iAmount) == true)
        {
            m_hashItemCount[_pSOData.Id] += _iAmount;
            m_pSlotContainer.UpdateItemCount(m_hashItemCount[_pSOData.Id]);
            return true;
        }

        return false;
    }

    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        //기존에 누른 데이터
        if (_pSOData == null)
            return false;

        var pListSlot = m_pSlotContainer.SlotList;
        if (pListSlot.Count <= _iDataIdx || m_hashItemCount.ContainsKey(_pSOData.Id))
            return false;


        //해당 데이터 사입 후 들어온 갯수 기록
        m_hashItemCount.TryAdd(_pSOData.Id, 0);
        m_hashItemCount[_pSOData.Id] += _iAmount;

        m_pSlotContainer.AddData(_iDataIdx, _pSOData, _iCategoryIdx);

        m_pSlotContainer.UnActiveSlot();

        return true;
    }
    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0)
    {
        SOEntryUI pData = GetData(_iDataIdx, _iCategoryIdx);
        if (pData == null)
            return false;

        int iDataId = pData.Id;
        m_hashItemCount[iDataId] -= _iAmount;

        if (m_hashItemCount[iDataId] <= 0)
        {
            delete(iDataId, _iDataIdx, _iCategoryIdx);
        }

        return true;
    }
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pData = GetData(_iDataIdx, _iCategoryIdx);
        if(pData == null)
            return false;


        delete(pData.Id, _iDataIdx, _iCategoryIdx);

        return true;
    }
    public bool DeleteData(SOEntryUI _pSOData, int _iAmount)
    {
        return false;
    }
    private void delete(int _iDataId, int _iDataIdx, int _iCategoryIdx = 0)
    {
        m_pSlotContainer.DeleteData(_iDataIdx, _iCategoryIdx);
        m_hashItemCount.Remove(_iDataId);
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx = 0) { return false; }

}
