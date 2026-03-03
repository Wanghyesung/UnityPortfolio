using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/AnimationSetting", fileName = "SOAnimSetting")]

public class SOAnimationSetting: SOSkillLogic
{
    public bool stopAnimation = false;
    public float speed = 1f;
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {
        _pSkillContext.animator.speed = stopAnimation ? 0f : 1f;

        return eSkillState.Success;
    }
}
