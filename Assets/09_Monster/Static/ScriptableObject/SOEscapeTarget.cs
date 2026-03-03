using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using STATE = INode.STATE;

[CreateAssetMenu(menuName = "SO/ActionNode/EscapeTarget")]

public class SOEscapeTarget : SONode
{
    public float m_fEscapeDistance = 6.0f;
    public float m_fEscapeTime = 3.0f;
    public float[] m_listChachAngle = { -90.0f, -60.0f, -30.0f, 0.0f, 30.0f, 60.0f, 90.0f };

    public override INode CreateRuntime()
    {
        return new EscapeTargetRunTime(this);
    }

    private class EscapeTargetRunTime : INode
    {
        private SOEscapeTarget m_pEscapeTarget;
        private float m_fCurTime = 0.0f;
        public EscapeTargetRunTime(SOEscapeTarget _pEscapeTarget)
        {
            m_pEscapeTarget = _pEscapeTarget;
            m_fCurTime = _pEscapeTarget.m_fEscapeTime;
        }


        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            Vector2 vTargetPos = new Vector2(_pBB.Target.position.x, _pBB.Target.position.z);
            Vector2 vSelfPos = new Vector2(_pBB.Self.transform.position.x, _pBB.Self.transform.position.z);

            float fDistance = Vector2.Distance(vSelfPos, vTargetPos);
            
            m_fCurTime += _fDT;
            if (m_fCurTime >= m_pEscapeTarget.m_fEscapeTime)
            {
                m_fCurTime = 0;

                Vector3 vDir = _pBB.Target.position - _pBB.Self.transform.position;
                vDir.Normalize();
                vDir.y = 0.0f;
                Vector3 vDefaultDir = -vDir.normalized;


                for (int i = 0; i < m_pEscapeTarget.m_listChachAngle.Length; ++i)
                {
                    float fAngle = m_pEscapeTarget.m_listChachAngle[i];
                    Quaternion qRot = Quaternion.Euler(0.0f, fAngle, 0.0f);
                    Vector3 vCurDir = qRot * vDefaultDir;
                    vCurDir.Normalize();

                    Vector3 vEscapePos = _pBB.Self.transform.position + vCurDir * m_pEscapeTarget.m_fEscapeDistance;

                    if (NavMesh.SamplePosition(vEscapePos, out NavMeshHit tHit,
                    1.0f, _pBB.Agent.areaMask) == true)
                    {
                        _pBB.AnimBridge.SetRun(true);
                        _pBB.Agent.SetDestination(tHit.position);
                        return STATE.SUCCESS;
                    }
                }

                return STATE.FAILED;
            }


            if (fDistance >= m_pEscapeTarget.m_fEscapeDistance)
                return STATE.FAILED;
            else
                return STATE.SUCCESS;
        }


    }
}