using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkillContext = SkillRunner.SkillContext;
using eSkillState = SkillRunner.eSkillState;
//[CreateAssetMenu(menuName = "SO/Profiles/Logic", fileName = "LogicProfile")]
public abstract class SOSkillLogic : ScriptableObject
{
    public abstract eSkillState UpdateSkill(SkillContext _pSkillContext);
}
