using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;
using SkillContext = SkillRunner.SkillContext;
[CreateAssetMenu(menuName = "SO/Profiles/Logic/Charge", fileName = "SOPlayerCharge")]
public class SOPlayerCharge : SOSkillLogic
{
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {
        _pSkillContext.chargeTime += Time.deltaTime;

        float fChargeTime = _pSkillContext.skill.RunSkill.Option.chargetime;
        float fRatio = _pSkillContext.chargeTime / fChargeTime;

        //지정된 시간까지 대기, 눌린상태가 아니면 종료
        if (fRatio>=1.0f || _pSkillContext.pressed == false)
        {
            for (int i = 0; i < _pSkillContext.chargeEvents.Count; ++i)
                _pSkillContext.chargeEvents[i].EndEvent();
            return eSkillState.Success;
        }

        for (int i = 0; i < _pSkillContext.chargeEvents.Count; ++i)
            _pSkillContext.chargeEvents[i].UpdateEvent(fRatio);
        
        return eSkillState.Waiting;
    }
}
