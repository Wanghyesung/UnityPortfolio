using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/CheckRange")]
public class SOCheckRange : SONode
{
    public bool m_bIsIn = false;
    public float m_fCheckRange = 6.0f;

    public override INode CreateRuntime()
    {
        return new CheckRangeRunTime(this);
    }

    private class CheckRangeRunTime : INode
    {
        private SOCheckRange m_pCheckRange = null;
        public CheckRangeRunTime(SOCheckRange _pCheckRange)
        {
            m_pCheckRange = _pCheckRange;
        }


        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            float fDistance = GlobalAction.GetDisttanceToVector2(_pBB.Self.transform.position, _pBB.Target.position);
            //안쪽으로 검사
            if (m_pCheckRange.m_bIsIn == true)
            {
                if (m_pCheckRange.m_fCheckRange > fDistance)
                    return STATE.SUCCESS;
                else
                    return STATE.FAILED;
            }
            else
            {
                if (m_pCheckRange.m_fCheckRange < fDistance)
                    return STATE.SUCCESS;
                else
                    return STATE.FAILED;
            }
        }



    }

}
