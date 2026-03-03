using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item/Effect/UpSpeed")]
public class SOUpMaxSpeedEffect : SOItemEffect
{
    public bool Reverse = false;
    public override void Apply(EffectContext _tEffectCnt)
    {
        GameObject pTarget = _tEffectCnt.pTarget;
        if (pTarget != null)
        {
            if (pTarget.TryGetComponent<ObjectInfo>(out var pStatus) == true)
            {
                if(Reverse == true)
                    pStatus.AddSpeed(_tEffectCnt.Value.Int * -1);
                else
                    pStatus.AddSpeed(_tEffectCnt.Value.Int);
            }
        }
    }

}
