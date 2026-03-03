using System.Collections;
using System.Collections.Generic;
using UnityEngine;



using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/LookAtPlayer")]
public class SOLookAtPlayer : SONode
{
    public override INode CreateRuntime()
    {
        return new LookRuntime();
    }

    private class LookRuntime : INode
    {
        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {

            _pBB.Target = GameManager.m_Instance.Player.transform;
            _pBB.Self.transform.LookAt(_pBB.Target);

            return STATE.SUCCESS;
        }
    }
}
