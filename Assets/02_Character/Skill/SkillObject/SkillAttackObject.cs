using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;



public class SkillAttackObject : SkillObject
{
    private bool m_bNearAttack = false;
    private float m_fMoveSpeed = 0.0f;
    private Vector3 m_vDir = Vector3.zero;

    [SerializeField] private AssetReferenceGameObject m_pHitEffectRef = null;
    private GameObject m_pHitEffect = null;

    [SerializeField] private float m_fStartAttackTime = 0.0f;
    [SerializeField] private float m_fEndAttackTime = float.MaxValue;

    [SerializeField] private int m_iAttackCount = 1;
    private int m_iCurAttackCount = 0;

    private AttackInfo m_pAttackInfo = null;

    [SerializeField] private SOAudio m_pSKillAudio = null;
    [SerializeField] private SOAudio m_pSKillHitAudio = null;
    private int m_iSfxIdx = -1;
    protected override void Awake()
    {
        base.Awake();
        m_pAttackInfo = new AttackInfo();

        if (m_pSKillAudio != null && m_iSfxIdx != -1)
            SoundManager.m_Instance.StopSfx(m_iSfxIdx);
    }

    public override void OnSpawn()
    {
        base.OnSpawn();

        m_iCurAttackCount = 0;
        if (m_fStartAttackTime <= 0.0f)
            StartAttack();
        else
            EndAttack();

        if (m_pSKillAudio != null)
            m_iSfxIdx = SoundManager.m_Instance.PlaySfx(m_pSKillAudio, transform);
    }
    public override void OnDespawn()
    {
        base.OnDespawn();

        m_bIsSkillActive = false;

        if(m_iSfxIdx != -1)
            SoundManager.m_Instance.StopSfx(m_iSfxIdx);
    }
    public void OnDisable()
    {
        m_fCurLifeTime = 0.0f;
        m_iCurAttackCount = 0;
    }

    protected override void Update()
    {
        base.Update();

        if(m_bIsSkillActive == false && m_fCurLifeTime >= m_fStartAttackTime)
            StartAttack();
    }


    public void FixedUpdate()
    {
        if (m_pRigidbody == null)
            return;

         Vector3 vStep = m_vDir.normalized * m_fMoveSpeed * Time.fixedDeltaTime;
         m_pRigidbody.MovePosition(m_pRigidbody.position + vStep);
    }

 
    public override void SetInfo(SOSKill _pSkillInfo, string _strKey)
    {
        base.SetInfo(_pSkillInfo, _strKey);
        m_fMoveSpeed = _pSkillInfo.Damage.moveSpeed;
        m_pAttackInfo.Damage = (int)_pSkillInfo.Damage.baseDamage;
        m_pAttackInfo.Power = (int)_pSkillInfo.Damage.power;
        m_pAttackInfo.AttackVariance = _pSkillInfo.Damage.damageVariance;

        m_iAttackCount = _pSkillInfo.Option.targetingProfile.MaxTargets;
        m_fStartAttackTime = _pSkillInfo.Damage.startAttackTime;
        m_bNearAttack = m_fMoveSpeed > 0.0f ? false : true;

        if(m_pOwner != null)
            m_vDir = m_pOwner.transform.forward;
    }

    public void SetDir(in Vector3 _vDir)
    {
        m_vDir = _vDir;
        m_vDir.Normalize();
    }


    public void OnTriggerEnter(Collider other)
    {
        if (m_pSkill == null)
            return;

        if ((m_pSkill.Option.targetingProfile.TargetLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            if (check_attack_count() == false)
                return;

            Vector3 vHitPoint = other.ClosestPoint(transform.position);
            Vector3 vAttackerPos = transform.position;

            if (m_bNearAttack == true)
                vAttackerPos = m_pOwner.transform.position;

            m_pAttackInfo.HitPoint = vHitPoint;
            m_pAttackInfo.HitPoint.y = 0.0f;
            m_pAttackInfo.AttackerPosition = vAttackerPos;
            m_pAttackInfo.HitSound = m_pSKillHitAudio;

            other.GetComponent<IHealth>()?.TakeDamage(m_pAttackInfo);

            StartEffect(vHitPoint);
            
        }
    }



    private bool check_attack_count()
    {
        ++m_iCurAttackCount;
        if (m_iCurAttackCount <= m_iAttackCount)
            return true;

        return false;
    }

    public void StartAttack()
    {
        if(m_pCollider == true)
            m_pCollider.enabled = true;
        m_bIsSkillActive = true;
    }
    public void EndAttack()
    {
        if (m_pCollider == true)
            m_pCollider.enabled = false;
        m_bIsSkillActive = false;
    }

    private void StartEffect(in Vector3 _vPoint)
    {
        if(m_pHitEffectRef == null)
            return;
        m_pHitEffect = ObjectPoolManager.m_Instance.GetObject(ePoolType.Global, m_pHitEffectRef.AssetGUID, _vPoint, Vector3.zero);
        ParticleCallback pCallback = m_pHitEffect.GetComponent<ParticleCallback>();
        if (pCallback != null)
            pCallback.SetCompletedAction(PushEffectPool);
    }
    private void PushEffectPool()
    {
        ObjectPoolManager.m_Instance.PushObject(ePoolType.Global, m_pHitEffectRef.AssetGUID, m_pHitEffect);
    }
}
