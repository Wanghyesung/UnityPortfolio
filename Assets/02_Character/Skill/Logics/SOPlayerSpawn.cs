using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static SkillRunner;
using SkillContext = SkillRunner.SkillContext;


[CreateAssetMenu(menuName = "SO/Profiles/Logic/PlayerSpawn", fileName = "SOPlayerSpawn")]
public class SOPlayerSpawn : SOSkillLogic
{

    public bool IsUseDesignPos = false;
    public AssetReference AttackObjectReference = null;
    public override eSkillState UpdateSkill(SkillContext _pSkillContext)
    {
        SkillRunner pPlayerSkill = _pSkillContext.skill;
        TargetingProfile pTargetPro = pPlayerSkill.RunSkill.Option.targetingProfile;

        Vector3 vSpawnPos = pPlayerSkill.gameObject.transform.position;
        Vector3 vRot = pPlayerSkill.gameObject.transform.eulerAngles;
        Vector3 vDir = pPlayerSkill.gameObject.transform.forward; 

        if(IsUseDesignPos == true)
            vSpawnPos = _pSkillContext.designSpawnPostion;

        else if (pTargetPro.playerFacing == true)
            vSpawnPos += (vDir * pTargetPro.spawnDistance);

        vSpawnPos+= pTargetPro.offset;

        GameObject pAttackObject = 
            ObjectPoolManager.m_Instance.GetObject(ePoolType.Global, AttackObjectReference.AssetGUID, vSpawnPos, Vector3.zero);

        //플레이어가 바라보는 방향
        Vector3 vObjectAngle = pAttackObject.transform.eulerAngles;
        vObjectAngle.y = vRot.y;
        vObjectAngle +=  pTargetPro.offsetRot;
        pAttackObject.transform.rotation = Quaternion.Euler(vObjectAngle);

        if (pAttackObject.TryGetComponent<SkillObject>(out SkillObject pSkillObj) == true)
        {
            //스킬 초기화
            pSkillObj.SetOwner(_pSkillContext.skill);
            pSkillObj.SetInfo(pPlayerSkill.RunSkill, AttackObjectReference.AssetGUID);
            pSkillObj.Init();

            _pSkillContext.runSkillObject = pSkillObj;
            _pSkillContext.chargeEvents = pSkillObj.ChargeEvents;

            for(int i = 0; i<_pSkillContext.chargeEvents.Count; ++i)
                _pSkillContext.chargeEvents[i].StartEvent();
            
            return eSkillState.Success;
        }
        else
        {
            ObjectPoolManager.m_Instance.PushObject(ePoolType.Global, AttackObjectReference.AssetGUID, pAttackObject);
            return eSkillState.Failed;
        } 
    }
}
