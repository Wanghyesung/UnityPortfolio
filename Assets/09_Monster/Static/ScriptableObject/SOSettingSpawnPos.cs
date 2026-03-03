using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;


[CreateAssetMenu(menuName = "SO/ActionNode/SettingSpawnPos")]


public class SOSettingSpawnPos : SONode
{
    public float m_fSpawnRadius = 5.0f;


    public override INode CreateRuntime()
    {
        return new SettingSpawnPosRuntime(this);
    }

    private class SettingSpawnPosRuntime : INode
    {
        private SOSettingSpawnPos _pOwner = null;

        public SettingSpawnPosRuntime(SOSettingSpawnPos pOwner)
        {
            _pOwner = pOwner;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            int iTargetIdx = _pBB.CooldownModule.TargetIdx;
            MonsterSkillInfo pSkillInfo = _pBB.Self.SOMonsterInfo.skillinfo[iTargetIdx];
            MonsterSkillOption pSkillOption = pSkillInfo.SkillOption;
            MonsterSkillSpawnOption pSpawnOption = pSkillInfo.SpawnOption;

            Vector3 vSpawnPos = Vector3.zero;
            Vector3 vSpawnRot = Vector3.zero;

            //위치 잡기
            if(pSpawnOption.SpawnPoint != ePointType.None)
            {
                Transform vPointTransform = _pBB.Self.GetMonsterPoint(pSpawnOption.SpawnPoint);
                vSpawnPos = vPointTransform.position;
                vSpawnRot = vPointTransform.rotation.eulerAngles;
            }
            else if (pSpawnOption.SpawnCurPos == true)
                vSpawnPos = _pBB.Self.transform.position;
            else if (pSpawnOption.SpawnPlayerPos == true)
                vSpawnPos = _pBB.Target.transform.position;
            
            // 랜덤 위치
            if(pSpawnOption.SpawnRandomPos == true)
            {
                Vector2 vRand = Random.insideUnitCircle * _pOwner.m_fSpawnRadius;
                vSpawnPos.x += vRand.x;
                vSpawnPos.z += vRand.y;
            }

            //방향 잡기
            Vector3 vDir = pSpawnOption.AttackDir;
            if (pSpawnOption.PlayerDir == true)
                vDir = (_pBB.Target.transform.position - vSpawnPos).normalized;
            else
                vDir = (_pBB.Self.transform.forward);

            vDir *= pSpawnOption.SpawnDiff;

            _pBB.AttackDir = vDir;
            _pBB.AttackSpawnPos = vSpawnPos;
            _pBB.AttackSpawnRot = vSpawnRot;
            return STATE.SUCCESS;
        }
    }
}