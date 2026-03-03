using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;


[CreateAssetMenu(menuName = "SO/ActionNode/WaitSpawnFrame")]

public class SOSpawnCompleted : SONode
{
    public override INode CreateRuntime()
    {
        return new SpawnCompletedRuntime();
    }

    private class SpawnCompletedRuntime : INode
    {
        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            if (_pBB.HitAnimationEvent == false)
                return STATE.RUN;

            _pBB.HitAnimationEvent = false;
            return STATE.SUCCESS;
        }
    }
}