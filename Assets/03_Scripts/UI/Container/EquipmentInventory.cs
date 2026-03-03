using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentInventory : BaseUI , IContainer
{
    [SerializeField] private SlotContainer m_pEquipSlotContainer = null;

    Dictionary<int, int> m_hashItemCount = new Dictionary<int, int>();

    [SerializeField] private eContainerType m_eContainerType = eContainerType.Equipment; 
    public eContainerType ContainerType { get => m_eContainerType; }


    [SerializeField] private List<GameObject> m_listOption = new List<GameObject>();
    [SerializeField] private List<TextMeshProUGUI> m_listText = new List<TextMeshProUGUI>();
    [SerializeField] private Stat m_pPlayerStat = null;

    public void OnClickOptionButton(GameObject _pTarget)
    {
        for (int i = 0; i < m_listOption.Count; ++i)
        {
            if (_pTarget == m_listOption[i])
            {
                _pTarget.SetActive(true);
                m_listText[i].color = new Color(0.575f, 0.333f, 0.035f, 1.0f);//갈색
            }
            else
            {
                m_listOption[i].SetActive(false);
                m_listText[i].color = Color.white;
            }
        }
    }

    //IContainer 구현
    public void Init()
    {
        m_pEquipSlotContainer.Init();
    }

    public void SetVisible(bool _bOn)
    {
        gameObject.SetActive(_bOn);
    }
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        return m_pEquipSlotContainer.GetData(_iDataIdx, _iCategoryIdx);
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
        int iAmount = 0;
        if (m_hashItemCount.TryGetValue(_pSoData.Id, out iAmount) == false)
            return 0;

        return iAmount;
    }

    public bool AddData(SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        if (_pSOData == null)
            return false;

        var listSlot = m_pEquipSlotContainer.SlotList;
        for(int i = 0; i< listSlot.Count; ++i)
        {
            if(listSlot[i].GetSlotHashCode() == _pSOData.GetUIHashCode())
            {
                return AddData(i, _pSOData, _iAmount);
            }
        }
        return false;
    }

    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        //기존에 누른 데이터
        if (_pSOData == null)
            return false;

        var listSlot = m_pEquipSlotContainer.SlotList;
        if (listSlot.Count <= _iDataIdx || m_hashItemCount.ContainsKey(_pSOData.Id))
            return false;


        //해당 데이터 사입 후 들어온 갯수 기록
        m_hashItemCount.TryAdd(_pSOData.Id, 0);
        m_hashItemCount[_pSOData.Id] += _iAmount;

        m_pEquipSlotContainer.AddData(_iDataIdx, _pSOData, _iCategoryIdx);

        m_pEquipSlotContainer.UnActiveSlot();

        // 장비 능력치 업데이트
        m_pPlayerStat?.UpdateStatText();

        return true;
    }
    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0)
    {
        return true;
    }
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pData = GetData(_iDataIdx, _iCategoryIdx);
        if (pData == null)
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
        m_pEquipSlotContainer.DeleteData(_iDataIdx, _iCategoryIdx);
        m_hashItemCount.Remove(_iDataId);
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx = 0) { return false; }


}
