using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWhileCasting : MonoBehaviour , IChargeEvent
{
   
    [SerializeField] private float m_fMoveSpeed = 2.0f;

    [SerializeField] private PED m_pCompleteEvent;
    [SerializeField] private PED m_pStartEvent;

    private SkillAttackObject m_pAttackObject = null;
    private SkillRunner m_pSkillRunner = null;

    private void Awake()
    {
        m_pAttackObject = GetComponent<SkillAttackObject>(); ;
    }

    public void StartEvent()
    {
        m_pSkillRunner = m_pAttackObject?.Owner;
        m_pStartEvent?.Invoke();
    }
    public void EndEvent()
    {
        m_pSkillRunner?.SetSpawnPosition(transform.position);
        m_pCompleteEvent?.Invoke();
    }

    public void UpdateEvent(float _fDeltaTime)
    {
        Vector2 vDir = InputManager.m_Instance.ActionState.vDirection;
        Vector2 vStep = (vDir * m_fMoveSpeed * _fDeltaTime);

        transform.position += new Vector3(vStep.x, 0.0f, vStep.y);
    }
   
}
