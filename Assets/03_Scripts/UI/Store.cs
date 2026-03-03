using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Store : BaseUI, IContainer
{

    [SerializeField] private ButtonUI m_pCloseButton = null;
    [SerializeField] private ButtonUI m_pPurchaseButton = null;
    [SerializeField] private Container m_pItemContainer = null;
    [SerializeField] private TextMeshProUGUI m_pCoinText = null;

    [SerializeField] private eContainerType m_eContainerType = eContainerType.Store; // ← 인스펙터에 드롭다운으로 보임
    public eContainerType ContainerType { get => m_eContainerType; }

    public void Init()
    {
        m_pItemContainer.SetParent(this);
        m_pItemContainer.Build();
    }
    public void SetVisible(bool _bOn)
    {
        gameObject.SetActive(_bOn);
    }
    protected override void Awake()
    {
        base.Awake();

        //close selete 함수 바인딩
        //m_pCloseButton.OnUpEvt += close_tap;

        m_pPurchaseButton.OnClickEvt += purchase_shapitem;
    }


    private void OnEnable()
    {
        update_coin();
    }
  
    private void close_tap()
    {
        //레이까지 제거하기 위해서
        gameObject.SetActive(false);
        m_pItemContainer.ClearTarget();
    }
  

    private void purchase_shapitem()
    {
        SlotView pTarget = m_pItemContainer.GetTargetSlot();
        if (pTarget == null)
            return;

        SOShopItem pShapItem = pTarget.SOEntryUI as SOShopItem;
        if (pShapItem != null)
        {
           //데이터 매니저에서 플레이어 코인 값 가져오기 가져왔다면 비교 후 DataService를 통해서 전달
           if(ShopManager.m_Instance.Spend(ShopManager.eCurrency.Coin, pShapItem.Coin) == true)
           {
                DataService.m_Instance.StartPickData(this, pShapItem.ItemUI, pTarget.SlotIdx, 1);

                DataService.m_Instance.TryAddData(eContainerType.Inventory);

                m_pItemContainer.ClearTarget();

                update_coin();
           }
        }
    }
    private void update_coin()
    {
        m_pCoinText.SetText("{0}", ShopManager.m_Instance.Get(ShopManager.eCurrency.Coin));
    }



    //IContainer 구현
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0) { }

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
        m_pItemContainer.ClearTarget();
        return true;
    }
    public bool DeleteData(SOEntryUI _pSOData, int _iAmount)
    {
        return false;
    }

    public bool FindData(SOEntryUI _pData, int _iCategoryIdx) { return false; }
    public bool FindData(int _iDataIdx, int _iCategoryIdx) { return false; }


}
