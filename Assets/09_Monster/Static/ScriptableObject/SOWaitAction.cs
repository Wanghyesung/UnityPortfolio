using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/WaitAction")]
public class SOWaitAction : SONode
{
    public float m_fWaitTime = 0.0f;
    public override INode CreateRuntime()
    {
        return new WaitActionRuntime(this);
    }

    private class WaitActionRuntime : INode
    {
        private float m_fCurTime;
        private SOWaitAction m_pOwner;
        public WaitActionRuntime(SOWaitAction _pOwner)
        {
            m_pOwner = _pOwner;
            m_fCurTime = 0.0f;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            m_fCurTime += _fDT;
            if(m_fCurTime >= m_pOwner.m_fWaitTime)
            {
                m_fCurTime = 0.0f;
                return STATE.SUCCESS;
            }

            return STATE.RUN;
        }
    }
}
