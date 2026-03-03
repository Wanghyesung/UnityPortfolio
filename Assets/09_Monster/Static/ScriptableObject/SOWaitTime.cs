using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/WaitTime")]

public class SOWaitTime : SONode
{
    public float m_fWaitTime = 0.0f;
    public override INode CreateRuntime()
    {
        return new WaitTimeRuntime(this);
    }

    private class WaitTimeRuntime : INode
    {
        private SOWaitTime m_pOwner;
        private float m_fCurTime;

        public WaitTimeRuntime(SOWaitTime _pOwner)
        {
            m_pOwner = _pOwner;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            m_fCurTime += _fDT;
            if (m_fCurTime >= m_pOwner.m_fWaitTime)
            {
                m_fCurTime = 0.0f;
                return STATE.SUCCESS;
            }

            return STATE.FAILED;
        }
    }

}
