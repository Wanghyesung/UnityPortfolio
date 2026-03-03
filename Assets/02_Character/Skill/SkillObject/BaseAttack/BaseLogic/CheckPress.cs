using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/CheckPress", fileName = "SOCheckPress")]

public class CheckPress : SOSkillLogic
{
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {   
        if(_pSkillContext.pressed == true)
            return eSkillState.Success;

        return eSkillState.Failed;
    }
   
}
