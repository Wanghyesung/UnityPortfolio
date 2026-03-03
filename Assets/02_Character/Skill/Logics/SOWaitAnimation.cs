using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/WaitAnimation", fileName = "SOWaitAnimation")]
public class SOWaitAnimation : SOSkillLogic
{
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {
        if(_pSkillContext.hitEvent == true)
        {
            _pSkillContext.hitEvent = false;
            return eSkillState.Success;
        }
        return eSkillState.Waiting;
    }
}
