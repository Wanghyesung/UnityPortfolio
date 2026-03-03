using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/UPDefend")]
public class SOUpDefendMon : SONode
{
    public int UpDefendValue = 0;
    public override INode CreateRuntime()
    {
        return new DefendUpRuntime(this);
    }

    private class DefendUpRuntime : INode
    {
        private SOUpDefendMon m_pOwner = null;

        public DefendUpRuntime(SOUpDefendMon _pOwner)
        {
            m_pOwner = _pOwner;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            _pBB.ObjectInfo.AddDefense(m_pOwner.UpDefendValue);
            return STATE.SUCCESS;
        }
    }
}
