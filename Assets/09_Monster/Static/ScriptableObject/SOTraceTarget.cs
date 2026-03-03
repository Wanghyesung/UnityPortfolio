using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/TraceTarget")]
public class TraceTarget : SONode
{
    public float m_fRepathTime = 0.25f;
    public float m_fStopDistance = 1.6f;
    public int m_iareaMask = NavMesh.AllAreas;

    public override INode CreateRuntime()
    {
        return new TraceTargetRuntime(this);
    }

    private class TraceTargetRuntime : INode
    {
        private TraceTarget m_pTraceTarget = null;
        private float m_fCurTime = 0.0f;
        public TraceTargetRuntime(TraceTarget _pOwner)
        {
            m_pTraceTarget = _pOwner;
            m_fCurTime = 0.0f;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            if (_pBB.Target == null)
                return STATE.FAILED;

            //거리 계산
            _pBB.DistanceToTarget = GlobalAction.GetDisttance(_pBB.Self.transform.position, _pBB.Target.position);

            //목적지에 도착했다면 
            if ( _pBB.DistanceToTarget <= m_pTraceTarget.m_fStopDistance)
            {
                _pBB.AnimBridge.SetRun(false);
                _pBB.Agent.ResetPath();
               
                return STATE.FAILED;
            }

            m_fCurTime += _fDT;

            //NavMeshAgent의 현재 경로가 잘못된 상태가 아니라면 일정 시간마다 목적지 재설정
            if (m_fCurTime >= m_pTraceTarget.m_fRepathTime ||
                _pBB.Agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                m_fCurTime = 0.0f;
                //NavMesh.SamplePosition으로 대상 좌표 근처의 유효한 NavMesh 위치를 찾고목적지 설정
                if (NavMesh.SamplePosition(_pBB.Target.position, out NavMeshHit tHit,
                    3.0f, m_pTraceTarget.m_iareaMask))
                {
                    _pBB.AnimBridge.SetRun(true);
                    _pBB.Agent.ResetPath();
                    _pBB.Agent.SetDestination(tHit.position);

                    return STATE.SUCCESS;
                }
            }

           

            return STATE.RUN;
        }


    }
}