using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/Defend")]

public class SODefend : SONode
{
    public float DefendTime = 0.0f;
    public override INode CreateRuntime()
    {
        return new DefendRuntime(this);
    }

    private class DefendRuntime : INode
    {
        private float m_fCurTime = 0.0f;
        private float m_fDefendTime = 0.0f; 
        public DefendRuntime(SODefend _pOwner)
        {
            m_fDefendTime = _pOwner.DefendTime;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            m_fCurTime += _fDT; 
            if(m_fCurTime >= m_fDefendTime)
            {
                m_fCurTime = 0.0f;
                _pBB.AnimBridge.SetDefend(false);
                return STATE.SUCCESS;
            }

            return STATE.RUN;
        }
    }
}
