using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using STATE = INode.STATE;


[CreateAssetMenu(menuName = "SO/ActionNode/MonsterWaitAnimEnd")]
public class SOMonsterWaitAnimEnd : SONode
{
    public override INode CreateRuntime()
    {
        return new WaitAnimEndRuntime(this);
    }

    private class WaitAnimEndRuntime : INode
    {
        private SOMonsterWaitAnimEnd m_pAttackTarget = null;
        private float m_fCurTime;

        public WaitAnimEndRuntime(SOMonsterWaitAnimEnd _pOwner)
        {
            m_pAttackTarget = _pOwner;
            m_fCurTime = 0.0f;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            //현재 공격 모션이 끝났다면
            if (_pBB.Target == null || _pBB.Attacking == false)
            {
                _pBB.AnimBridge.SetAttack(_pBB.CooldownModule.TargetIdx, false);
                _pBB.AnimBridge.m_pAnimator.speed = 1.0f;
                _pBB.Agent.updateRotation = true;
                return STATE.SUCCESS;
            }

            else
                return STATE.RUN;
        }
    }
}