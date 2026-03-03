using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using STATE = INode.STATE;


[CreateAssetMenu(menuName = "SO/ActionNode/SpawnMonsterAttack")]

public class SOSpawnMonsterAttack : SONode
{
    public override INode CreateRuntime()
    {
        return new SpawnMonsterAttackRuntime();
    }

    private class SpawnMonsterAttackRuntime : INode
    {
      
        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            int iTargetIdx = _pBB.CooldownModule.TargetIdx;

            //몬스터 스킬 정보를 통해서 타겟 몬스터 어택 오브젝트 레퍼런스의 아이디를 가져오기
            MonsterSkillInfo pSkillInfo = _pBB.Self.SOMonsterInfo.skillinfo[iTargetIdx];
            string strKey = pSkillInfo.SpawnOption.SOPoolEntry.prefabRef.AssetGUID;

            GameObject pAttackObj = ObjectPoolManager.m_Instance.GetObject(ePoolType.Stack,
                strKey, _pBB.AttackSpawnPos + _pBB.AttackDir, _pBB.AttackSpawnRot);

            if (pAttackObj.TryGetComponent<MonsterAttackObject>(out var pAttack) == false)
            {
                ObjectPoolManager.m_Instance.PushObject(ePoolType.Stack, strKey, pAttackObj);
                return STATE.FAILED;
            }

            pAttack.SetInfo(pSkillInfo);
            pAttack.SetDir(_pBB.AttackDir);
            pAttack.SetOwner(_pBB.Self);

            //근접공격은 피격시 사라지고, 소환공격은 계속 유지되게
            if (pSkillInfo.SkillOption.DestroyOn == true)
                _pBB.SpawnObjects.Add(pAttack);

            return STATE.SUCCESS;
        }
    }
}