using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/ConditionTrigger", fileName = "SOConditionTrigger")]
public class ConditionAnimTrigger : SOSkillLogic
{
    public bool IsPressed = false;
    public string triggerName;
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {
        if (_pSkillContext.pressed == true)
        {
            if (IsPressed == true)
                _pSkillContext.animator.SetTrigger(triggerName);

             return eSkillState.Success;
        }
        return eSkillState.Failed;
    }
}