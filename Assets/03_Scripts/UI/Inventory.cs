using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static SOEntryUI;


public class Inventory : BaseUI, IContainer
{
    enum eInventoryButton
    {
        //장착
        None,
        Equippped,
        AutoEquipped,
        End,
    }

    [SerializeField] private eContainerType m_eContainerType = eContainerType.Inventory; // ← 인스펙터에 드롭다운으로 보임
    public eContainerType ContainerType { get => m_eContainerType; }
    
    [SerializeField] private SlotContainer m_pInterfaceSlotContainer;
    [SerializeField] private SlotContainer m_pEquipSlotContainer;

    [SerializeField] private Container m_pInevenContainer;

    [SerializeField] private List<eUIType> m_listCategoryType;
    [SerializeField] private List<ButtonUI> m_listCategoryButton;
    [SerializeField] private List<ButtonUI> m_listOptionButton = new List<ButtonUI>();
    [SerializeField] private Stat m_pPlayerStat = null;

    //인벤에서 인터페이스 , 장비창
    Dictionary<uint, int> m_hashItemCount = new Dictionary<uint, int>();
    Dictionary<uint, CategoryData> m_hashCategoryData = new Dictionary<uint, CategoryData>();

    //Test

    [SerializeField] private Color m_pBaseColor;

    //DataService 에서 요청할 때 사용하는 인터페이스


    public void Init()
    {
        m_pInevenContainer.SetParent(this);

        int testValue = -1;
        for (int i = 0; i < m_pInevenContainer.CategoryCount; ++i)
        {
            testValue += 2;
            CategoryData pCategoryData = m_pInevenContainer.GetCategoryData(i);
            List<SOEntryUI> ListData = pCategoryData.m_ListData;

            for (int j = 0; j < ListData.Count; ++j)
            {
                if (ListData[j] == null)
                    continue;

                 
                if (m_hashItemCount.TryGetValue((uint)ListData[j].Id, out int iCurCount))
                    m_hashItemCount[(uint)ListData[j].Id] += testValue;
                else
                    m_hashItemCount.Add((uint)ListData[j].Id, testValue);
            }
        }


        m_pInevenContainer.Build();

    }

    public void SetVisible(bool _bOn)
    {
        gameObject.SetActive(_bOn);
    }
    override protected void Awake()
    {
        base.Awake();
        
        m_pInevenContainer.OnSelectEvt += select;


        button_option();

    }
    protected void Start()
    {
       
    }

    public void Update()
    {
        
    }

    private void OnDisable()
    {
        m_pEquipSlotContainer?.UnActiveSlot();
    }


    public int GetCategoryIdx(eUIType _eUIType)
    {
        for (int i = 0; i < m_listCategoryType.Count; ++i)
        {
            if (m_listCategoryType[i] == _eUIType)
                return i;
        }
        return -1;
    }
  
    public void AddDataInventroy(SOEntryUI _pData, int _iAmount)
    {
        uint iUITypeCode = _pData.GetUIHashCode();
    }

    private void select()
    {
        SlotView pTargetView = m_pInevenContainer.GetTargetSlot();
        if (pTargetView.SOEntryUI == null)
            return;

        //해당 아이템 별 분기처리
        switch(pTargetView.SOEntryUI.Type)
        {
            case eUIType.Item:
                select_item();
                break;
            case eUIType.Equip:
                select_equip();
                break;
            default:
                return;
        }
    }

    private void select_item()
    {
        //전송할 데이터 예약
        SlotView pTargetView = m_pInevenContainer.GetTargetSlot();
        int iAmount = m_hashItemCount[(uint)pTargetView.SOEntryUI.Id];
        DataService.m_Instance.StartPickData(this, pTargetView.SOEntryUI, pTargetView.SlotIdx, iAmount, GetCategoryIdx(eUIType.Item));


        //인터페이스로 해당 내 아이템 전송 요청
        int iItemIdx= m_pInterfaceSlotContainer.GetSlotIdx(eUIType.Item);
        m_pInterfaceSlotContainer.ActiveSlotAndAddData(pTargetView.SOEntryUI.GetUIHashCode(), iItemIdx);   
    }

    private void select_equip()
    {
        SlotView pTargetView = m_pInevenContainer.GetTargetSlot();
       
        //중복 허용하지 않는다면 무조건 1
        int iCount = 1;
     
        //데이터 서비스에서 지금 눌린 데이터 참조
        DataService.m_Instance.StartPickData(this, pTargetView.SOEntryUI, pTargetView.SlotIdx, iCount);

        m_pEquipSlotContainer.UnActiveSlot();
        m_pEquipSlotContainer.ActiveSlot(pTargetView.SOEntryUI.GetUIHashCode());
    }

    private void equipped()
    {
        m_pInevenContainer.ChanageSelect();

        ButtonUI pButton =  m_listOptionButton[(int)eInventoryButton.Equippped];
        if (m_pInevenContainer.IsOnSelect)
            pButton.m_pTextMeshProUGUI.color = Color.yellow;
        else
            pButton.m_pTextMeshProUGUI.color = m_pBaseColor;
    }

    private void auto_equipped()
    {
        int equipCategoryIdx = m_pInevenContainer.GetCategoryIdx(eUIType.Equip);
        List<SOEntryUI> listEquip = m_pInevenContainer.GetListData(equipCategoryIdx);
        List<Slot> listCurrentEquip = m_pEquipSlotContainer.SlotList;

        Dictionary<int, EquipSlot> hashEquipSlot = new Dictionary<int, EquipSlot>(listCurrentEquip.Count);

        for (int j = 0; j < listCurrentEquip.Count; ++j)
        {
            EquipSlot pEquipSlot = listCurrentEquip[j] as EquipSlot;
          
            int iSlotHash = (int)pEquipSlot.GetSlotHashCode();
            if (hashEquipSlot.ContainsKey(iSlotHash) == false)
                hashEquipSlot.Add(iSlotHash, pEquipSlot);
        }

        for (int i = 0; i < listEquip.Count; ++i)
        {
            SOEntryUI entry = listEquip[i];
            if (entry == null)
                continue;

            SOEquipUI equipUI = entry as SOEquipUI;
            int iEquipHash = (int)equipUI.GetUIHashCode();

            EquipSlot pTargetSlot;
            if (hashEquipSlot.TryGetValue(iEquipHash, out pTargetSlot) == false)
                continue;

            uint currentLev = (pTargetSlot.SOEquip == null) ? 0u : pTargetSlot.SOEquip.ItemData.Level;
            uint equipLev = equipUI.ItemData.Level;

            if (equipLev > currentLev)
            {
                // 여기서 바로 실행하면 리스트/슬롯 상태가 변할 수 있음
                DataService.m_Instance.StartPickData(this, equipUI, i, 1, equipCategoryIdx);
                DataService.m_Instance.TryDropDataAndSwap(eContainerType.Equipment, pTargetSlot.SlotIdx);
            }
        }
        m_pPlayerStat?.UpdateStatText();
    }

    private void button_option()
    {
        //기존 텍스쳐 색상 캐싱
        TextMeshProUGUI pTextMeshPro = m_listCategoryButton[0].GetComponentInChildren<TextMeshProUGUI>();
        Color TextColor = pTextMeshPro.color;
        pTextMeshPro.color = Color.yellow;

        //카테고리 버튼
        for (int i = 0; i < m_listCategoryType.Count; ++i)
        {
            CategoryData pCategoryData = m_pInevenContainer.GetCategoryData(i);
            if (pCategoryData == null)
                break;

            m_hashCategoryData.Add((uint)m_listCategoryType[i], pCategoryData);

            int idx = i; //캡처용 복사본
            m_listCategoryButton[i].OnClickEvt += () =>
            {
                //색상 변경
                for (int j = 0; j < m_listCategoryButton.Count; ++j)
                {
                    if (j == idx)
                        m_listCategoryButton[j].m_pTextMeshProUGUI.color = Color.yellow;
                    else
                        m_listCategoryButton[j].m_pTextMeshProUGUI.color = TextColor;
                }

                m_pInevenContainer.ChanageCategory(idx);
            };
        }

        //인벤토리 옵션 버튼
        m_listOptionButton[(int)eInventoryButton.Equippped].OnClickEvt += equipped;
        m_listOptionButton[(int)eInventoryButton.AutoEquipped].OnClickEvt += auto_equipped;
    }

    //IContainer 구현
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        return m_pInevenContainer.GetDataIdx(_iDataIdx, _iCategoryIdx);
    }
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0)
    {
        SOEntryUI pEntryData = GetData(_iDataIdx, _iCategoryIdx);
        if (pEntryData == null)
            return 0;

        if (m_hashItemCount.TryGetValue((uint)pEntryData.Id, out int iAmount) == false)
            return 0;

        if (m_pInevenContainer.GetCategoryData(_iCategoryIdx).IsCanDuplication == false)
            return 1;

        return iAmount;
    }
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0)
    {
        if (_pSoData == null)
            return 0;

        if (m_hashItemCount.TryGetValue((uint)_pSoData.Id, out int iAmount) == false)
            return 0;

        if (m_pInevenContainer.GetCategoryData(_iCategoryIdx).IsCanDuplication == true)
            return 1;

        return iAmount;
    }

    //비어있는 칸으로 넣어주기
    public bool AddData(SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        int iCategoryIdx = GetCategoryIdx(_pSOData.Type);
        if (iCategoryIdx == -1)
            return false;

        CategoryData pCategoryData = m_pInevenContainer.GetCategoryData(iCategoryIdx);
        uint iID = (uint)_pSOData.Id;


        if (pCategoryData.IsCanDuplication == false &&
            m_hashItemCount.TryGetValue((uint)_pSOData.Id, out int iCount) == true)
        {
            m_hashItemCount[iID] += _iAmount;
            return true;
        }
        else
        {
            //들어왔을 때 아이템이 슬롯 인터페이스에 있는지 확인
            if (DataService.m_Instance.TryAddData(eContainerType.Interface, _pSOData, _iAmount) == true)
                return true;

            m_hashItemCount.Add(iID, _iAmount);
            return m_pInevenContainer.AddData(_pSOData, iCategoryIdx);
        }
    }

    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0)
    {
        if(_iCategoryIdx == 0)
            _iCategoryIdx = GetCategoryIdx(_pSOData.Type);
        
        uint iID = (uint)_pSOData.Id;

        if (m_hashItemCount.TryGetValue(iID, out int iCount) == true)
            m_hashItemCount[iID] += _iAmount;
        else
            m_hashItemCount.Add(iID, _iAmount);

        m_pInevenContainer.AddData(_pSOData, _iCategoryIdx, _iDataIdx);

        return true;
    }


    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0) { return false; }

    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0)
    {
        //1.만약 바꾸는거라면 슬롯에 있는 데이터를 다시 인벤토리에 넣어야함
        //2.컨테이너에서 중복으로 감시하기 때문에 거기서 반환 값을 통해서 감시하기 굳이 인벤토리에서도 있는지 체크하지말기
        SOEntryUI pTargetData = m_pInevenContainer.GetDataIdx(_iDataIdx, _iCategoryIdx);
        if (pTargetData == null)
            return false;

        if (m_hashItemCount.ContainsKey((uint)pTargetData.Id) == false)
            return false;

        m_hashItemCount.Remove((uint)pTargetData.Id);
        m_pInevenContainer.DeleteData(_iDataIdx, _iCategoryIdx);
        m_pInevenContainer.ClearTarget();
        return true;
    }

    public bool DeleteData(SOEntryUI _pSOData, int _iAmount)
    {
        int iCategoryIdx = m_pInevenContainer.GetCategoryIdx(_pSOData.Type);
        int iDataIdx = m_pInevenContainer.GetDataIdx(_pSOData, iCategoryIdx);
        if (iDataIdx == -1)
            return false;

        m_hashItemCount[(uint)_pSOData.Id] -= _iAmount;
        if (m_hashItemCount[(uint)_pSOData.Id]<=0)
        {
            m_hashItemCount.Remove((uint)_pSOData.Id);
            m_pInevenContainer.DeleteData(iDataIdx, iCategoryIdx);
        }

        return true;
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx = 0) { return false; }


}
