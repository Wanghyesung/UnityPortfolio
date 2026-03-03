using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Effect
{
    [CreateAssetMenu(menuName = "SO/Item/Effect/UpAttack")]
    public class SOUpAttack : SOItemEffect
    {
        public bool Reverse = false;
        public override void Apply(EffectContext _tEffectCnt)
        {
            if(_tEffectCnt.pTarget == null)
                return;

            if (_tEffectCnt.pTarget.TryGetComponent<ObjectInfo>(out var pStatus) == true)
            {
                if(Reverse == true)
                    pStatus.UpAttack(_tEffectCnt.Value.Int * -1);
                else
                    pStatus.UpAttack(_tEffectCnt.Value.Int);
            }
               
        }


    };   
}

