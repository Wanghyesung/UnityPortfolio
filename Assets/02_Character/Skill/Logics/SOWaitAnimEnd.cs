using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/WaitAnimEnd", fileName = "SOWaitAnimEnd")]
public class SOWaitAnimEnd : SOSkillLogic
{
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {
        int iLayer = _pSkillContext.skill.RunSkill.Animation.layerIndex;
        var tInfo = _pSkillContext.animator.GetCurrentAnimatorStateInfo(iLayer);

        //현재 진행중인 애니메이션 끝났거나 다른 애니메이션으로 전환된경우
        if (tInfo.normalizedTime >= 0.95f) //1.0f -> 0.95f로 변경(끝나는 시점에 약간의 여유를 주기위해)
            return eSkillState.Success;

        return eSkillState.Waiting;
    }
}
