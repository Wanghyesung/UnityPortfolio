using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IItemUI
{
    SOItem ItemData { get; }
}

[CreateAssetMenu(menuName = "UIData/Catalog/Item UI", fileName = "SOEntryUI")]
public class SOItemUI : SOEntryUI , IItemUI
{
    public enum eItemType
    {
        None,
        ConsumeItem,
        Quest,
    }

    [SerializeField] private eItemType itemtype;
    [SerializeField] private SOItem itemdata;  

    public eItemType ItemType => itemtype;

    public SOItem ItemData => itemdata;

    public override uint GetUIHashCode()
    {
        uint iHashCode = base.GetUIHashCode();
        iHashCode |= (uint)itemtype << (int)SOEntryUI.eUIType.Item;

        return iHashCode;
    }
}
