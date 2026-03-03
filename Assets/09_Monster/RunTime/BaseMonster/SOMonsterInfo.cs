using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum eMonsterType
{
    Normal,
    Boss,
}


[Serializable]
public class MonsterSkillInfo
{
    public MonsterSkillOption SkillOption = null;
    public MonsterSkillSpawnOption SpawnOption = null;
}


[CreateAssetMenu(menuName = "SO/Monster/MonsterInfo")]
public class SOMonsterInfo : ScriptableObject
{
    public int MonsterID;
    public int MonsterLevel;
  
    public eMonsterType MonsterType;
    //뭐 나올지
    //몬스터 기본 옵션
    //public SOMonsterSpawnOption MonsterSpawnOption = null;
    public SODropTable DropTable = null;
    public List<MonsterSkillInfo> skillinfo = new List<MonsterSkillInfo>();
}
