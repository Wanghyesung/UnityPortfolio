using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UIData/Catalog/ShapItem UI", fileName = "ShapItemUI")]

public class SOShopItem : SOEntryUI
{
    [SerializeField] private SOEntryUI itemui;
    public SOEntryUI ItemUI => itemui;

    [SerializeField] private uint coin;
    public uint Coin => coin;

    private void OnValidate()
    {
        icon = itemui.Icon;
    }
}
