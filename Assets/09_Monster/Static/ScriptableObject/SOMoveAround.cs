using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using STATE = INode.STATE;

[CreateAssetMenu(menuName = "SO/ActionNode/MoveAround")]
public class SOMoveAround : SONode
{
    public float m_fChangeDirTime = 2.0f;
    public float m_fStepDistance = 3.0f;
    public override INode CreateRuntime()
    {
        return new MoveAroundRunTime(this);
    }

    private class MoveAroundRunTime : INode
    {
        private SOMoveAround m_pMoveAround;
      
        private float m_fChangeDirTime = 0.0f;

        private Vector2 m_vRandomDir = Vector2.zero;
        public MoveAroundRunTime(SOMoveAround _pMoveAround)
        {
            m_pMoveAround = _pMoveAround;
            m_fChangeDirTime = _pMoveAround.m_fChangeDirTime;
        }
        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            m_fChangeDirTime += _fDT;
            if (m_fChangeDirTime >= m_pMoveAround.m_fChangeDirTime)
            {
                m_fChangeDirTime = 0f;
                m_vRandomDir = RandomUnit2D();
                _pBB.AnimBridge.SetRun(true);
            }
            else
                return STATE.FAILED;

            float fStep = m_pMoveAround.m_fStepDistance; 
            
            Vector3 vNewPos = _pBB.Self.transform.position + new Vector3(m_vRandomDir.x, 0f, m_vRandomDir.y) * fStep;

            if (NavMesh.SamplePosition(vNewPos, out var hit, 0.1f, _pBB.Agent.areaMask))
            {
                _pBB.Agent.SetDestination(hit.position);
                return STATE.SUCCESS;
            }
            else
            {
                m_vRandomDir = Vector2.zero;
                _pBB.AnimBridge.SetRun(false);
                _pBB.Agent.ResetPath();
                return STATE.FAILED;
            }
            
        }
       
        private Vector2 RandomUnit2D()
        {
            // Mathf 사용 버전: 
            float ang = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));

        }
    }


   

}
