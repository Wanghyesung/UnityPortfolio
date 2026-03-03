using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;


[CreateAssetMenu(menuName = "SO/Profiles/Logic/TransferPlayerChild", fileName = "SOTransferPlayerChild")]
public class SOTransferPlayerChild : SOSkillLogic
{
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {
        SkillObject pSkillObj = _pSkillContext.runSkillObject;
        if (pSkillObj == null)
            return eSkillState.Failed;

        //월드 위치/회전을 유지하고 자식으로 편입
        pSkillObj.transform.SetParent(_pSkillContext.skill.transform,true);
        return eSkillState.Success;
    }
}


