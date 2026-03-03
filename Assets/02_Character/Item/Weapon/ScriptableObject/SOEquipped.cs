using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item/Effect/Equipped")]


public class SOEquipped : SOItemEffect
{
    public eEquipType EquipType;
    public override void Apply(EffectContext _tEffectCnt)
    {
        Player pPlayer = _tEffectCnt.pTarget.GetComponent<Player>();
        if (pPlayer == null || _tEffectCnt.pOwner == null)
            return;

        Transform pPointTransform = pPlayer.GetEquipPoint(EquipType);
        _tEffectCnt.pOwner.transform.SetParent(pPointTransform, false);
        _tEffectCnt.pOwner.transform.localPosition = Vector3.zero;
    }


}
