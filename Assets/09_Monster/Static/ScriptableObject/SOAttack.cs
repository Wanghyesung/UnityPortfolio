using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/Attack")]
public class SOAttack : SONode
{
    public int AttackID = -1;
    public override INode CreateRuntime()
    {
        return new AttacRuntime(this);
    }

    private class AttacRuntime : INode
    {
        private int m_iAttackID = -1;
        public AttacRuntime(SOAttack _pOwner)
        {
            m_iAttackID = _pOwner.AttackID;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            //쿨타임이 다 되지 않았다면
            if (_pBB.Target == null || _pBB.Attacking == true) 
                return STATE.FAILED;
            else if(_pBB.CooldownModule.IsIsReady(m_iAttackID) == false)
                return STATE.FAILED;

            _pBB.Attacking = true;
            _pBB.Agent.ResetPath();
            _pBB.Agent.updateRotation = false;

            Debug.Log($"공격시작 {m_iAttackID}");
            //쿨타임 초기화 및 어택 시작
            _pBB.CooldownModule.StartCooldown(m_iAttackID);
            _pBB.AnimBridge.SetRun(false);
            _pBB.AnimBridge.SetAttack(m_iAttackID, true);
            //_pBB.Self.transform.LookAt(_pBB.Target.position);

            return STATE.SUCCESS;
        }
    }
}
