using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item/Effect/DownAttack")]
public class SODownAttack : SOItemEffect
{
    public override void Apply(EffectContext _tEffectCnt)
    {
        if (_tEffectCnt.pTarget == null)
            return;

        if (_tEffectCnt.pTarget.TryGetComponent<ObjectInfo>(out var pStatus) == true)
            pStatus.UpAttack(_tEffectCnt.Value.Int * -1);
    }


}
