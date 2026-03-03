using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/CheckFOV")]

public class SOCheckFOV : SONode
{
    public float m_fFOV = 90f;
  
    public override INode CreateRuntime()
    {
        return new CheckFOVRunTime(this);
    }

    private class CheckFOVRunTime : INode
    {
        private SOCheckFOV m_pFindTarget = null;
        public CheckFOVRunTime(SOCheckFOV _pOwner)
        {
            m_pFindTarget = _pOwner;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            if (_pBB.Agent == null || _pBB.Target == null)
                return STATE.FAILED;

            float fAngle =
                GlobalAction.GetDirection(_pBB.Self.transform.forward, _pBB.Self.transform.position, _pBB.Target.position);
            if (fAngle > m_pFindTarget.m_fFOV * 0.5f)
                return STATE.FAILED;

            return STATE.SUCCESS;
        }
    }

}
