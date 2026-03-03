using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/SettingAnimSpeed")]

public class SOSetAnimSpeed : SONode
{
    public float Speed = 1.0f;
    public override INode CreateRuntime()
    {
        return new SetAnimSpeedRuntime(this);
    }

    private class SetAnimSpeedRuntime : INode
    {
        SOSetAnimSpeed m_pOwner = null;

        public SetAnimSpeedRuntime(SOSetAnimSpeed pOwner)
        {
            m_pOwner = pOwner;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            _pBB.AnimBridge.m_pAnimator.speed = m_pOwner.Speed;
            
            return STATE.SUCCESS;
        }
    }
}