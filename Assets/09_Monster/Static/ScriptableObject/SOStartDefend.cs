using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/StartDefend")]
public class SOStartDefend : SONode
{
    public int UpDefendValue = 0;
    public override INode CreateRuntime()
    {
        return new StartDefendRuntime(this);
    }

    private class StartDefendRuntime : INode
    {
        private SOStartDefend m_pOwner = null;
       
        public StartDefendRuntime(SOStartDefend _pOwner)
        {
            m_pOwner = _pOwner;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            _pBB.AnimBridge.SetDefend(true);
            return STATE.SUCCESS;
        }
    }
}
