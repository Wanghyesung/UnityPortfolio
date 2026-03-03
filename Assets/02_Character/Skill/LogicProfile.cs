using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Profiles/LogicProfile", fileName = "LogicProfile")]
public class LogicProfile : ScriptableObject
{
    public List<SOSkillLogic> skillLogic;
    public SOSkillLogic endSkillLogic;
    public SOSkillLogic startSkillLogic;
}

