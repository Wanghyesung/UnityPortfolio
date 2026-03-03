using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SOItemUI;

[CreateAssetMenu(menuName = "UIData/Catalog/Equip UI", fileName = "SOEntryUI")]


public class SOEquipUI : SOEntryUI, IItemUI
{

    [SerializeField] private SOItem itemdata;
    [SerializeField] private eEquipType equiptype;
    public eEquipType EquipType => equiptype;

    public SOItem ItemData => itemdata;

    public override uint GetUIHashCode()
    {
        uint iHashCode = base.GetUIHashCode();
        iHashCode |= (uint)equiptype << (int)SOEntryUI.eUIType.Equip;

        return iHashCode;
    }
}
