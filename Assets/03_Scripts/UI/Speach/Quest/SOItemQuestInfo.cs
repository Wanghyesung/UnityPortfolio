using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/NPC/ItemQuestInfo")]
public class SOItemQuestInfo : SOQuestInfo
{
    public SOItem Item;
    private void OnValidate()
    {
        TargetId = Item.ItemID;
    }
}