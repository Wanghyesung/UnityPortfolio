using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillRunner;
using SkillContext = SkillRunner.SkillContext;

[CreateAssetMenu(menuName = "SO/Profiles/Logic/PlayerLock", fileName = "SOPlayerLock")]

public class SOPlayerLock : SOSkillLogic
{
    public bool bLockVelocity = false;
   
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {
        Rigidbody pPlayerRigid = _pSkillContext.skill.OwnerPlayer.RigidBody;

        if (bLockVelocity)
        {
            pPlayerRigid.velocity = Vector3.zero;
            pPlayerRigid.angularVelocity = Vector3.zero;
        }

        return eSkillState.Success;
    }
}
