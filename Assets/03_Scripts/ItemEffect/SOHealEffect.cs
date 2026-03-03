using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Item/Effect/Heal")]
public class SOHealEffect : SOItemEffect
{
    public override void Apply(EffectContext _tEffectCnt)
    {
        GameObject pTarget = _tEffectCnt.pTarget;
        if(pTarget != null)
        {
            if(pTarget.TryGetComponent<ObjectInfo>(out var pStatus) == true)
            {
                pStatus.AddHP(_tEffectCnt.Value.Int);
                HealthManager.m_Instance.UpdateHP();
            }
        }
    }

}

