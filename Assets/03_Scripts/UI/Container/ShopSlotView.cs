using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;

public class ShopSlotView : SlotView
{
        
    [SerializeField] private TextMeshProUGUI m_pNameText = null;
    [SerializeField] private TextMeshProUGUI m_pCoinText = null;
    private uint m_iCoin = 0;
    public uint Coin { get => m_iCoin; }

    public override void Bind(SOEntryUI _pEntryUI, int _iSlotIdx)
    {
        BindData(_pEntryUI, _iSlotIdx);

        SOShopItem pShopItem = _pEntryUI as SOShopItem;
        update_coin(pShopItem);
    }

    override public void OnBeginDrag(PointerEventData e)
    {
        base.OnBeginDrag(e);
    }

    override public void OnDrag(PointerEventData e)
    {
        base.OnDrag(e);
    }

    override public void OnEndDrag(PointerEventData e)
    {
        base.OnEndDrag(e);
    }

    public override void OnPointerClick(PointerEventData e)
    {
        if (m_pContainer.IsOnSelect == false)
            return;

        if (m_pContainer != null)
        {
            m_pContainer.SetTargetSlot(this);
        }
    }

    private void update_coin(SOShopItem _pShopData)
    {
        if (_pShopData == null)
        {
            m_pNameText.text = "";
            m_pCoinText.SetText("0");
        }
        else
        {
            //m_pNameText.text = _pShopData.Name;
            LocalizeStringEvent pStringEvent = m_pNameText.GetComponent<LocalizeStringEvent>();
            pStringEvent.StringReference = _pShopData.String;
            m_pCoinText.SetText("{0}", _pShopData.Coin); ;
        }
    }

}
