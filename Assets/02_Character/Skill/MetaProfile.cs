using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Profiles/Meta", fileName = "MetaProfile")]
public class MetaProfile : ScriptableObject
{
    public int skillid;                          // 스킬 ID
    public string skillName;                     // 스킬 이름
    [TextArea] public string description;        // 스킬 설명  
    public int levelRequirement;                 // 레벨 요구사항
}
