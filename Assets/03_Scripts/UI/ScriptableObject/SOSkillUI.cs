using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using eSkillType = SkillRunner.eSkillType;
[CreateAssetMenu(menuName = "UIData/Catalog/Skill UI", fileName = "SOEntryUI")]
public class SOSkillUI : SOEntryUI
{
   
    [SerializeField] private uint level;
    [SerializeField] private eSkillType skilltype;
    [SerializeField] private SOSKill skill;

    public uint Level => level;
    public eSkillType SkillType => skilltype;

    public SOSKill Skill => skill;
    
    public override uint GetUIHashCode()
    {
        uint iHashCode = base.GetUIHashCode();
        iHashCode |= (uint)skilltype << (int)SOEntryUI.eUIType.Skill;

        return iHashCode;
    }
}
