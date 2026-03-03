using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System;

[Serializable]
public enum ePointType
{
    None,
    Head,
    RightHand,
    LeftHand
}

[Serializable]
public class MonsterPoint
{
    public ePointType ePointType;
    public Transform pTrasnform;
}

public class Monster : MonoBehaviour, IHealth, IPoolAble
{
    [SerializeField] protected Blackboard m_pBlackbard = new Blackboard();
    private BehaviorTree m_pBHTree = null;
    [SerializeField] protected SOMonsterInfo m_SOMonsterInfo = null;
    [SerializeField] protected List<MonsterPoint> m_listPoint = new();
    [SerializeField] protected MonsterHPBar m_pMonsterHPBar = null;

    protected Rigidbody m_pRigidbody = null;
    protected Animator m_pAnimator = null;
    protected NavMeshAgent m_pNavMeshAgent = null;
    protected Collider m_pCollider = null;
    public SOMonsterInfo SOMonsterInfo => m_SOMonsterInfo;

    protected CooldownModule m_pCollDownModule = null;
    protected ObjectInfo m_pMonsterInfo = null;

    private Coroutine m_pKnockbackRoutine = null;

    private string m_strPoolKey = "";
    
    protected bool m_bHit = false;
    //IHealth
    public int CurrentHP => m_pMonsterInfo.HP;
    public int MaxHP => m_pMonsterInfo.MaxHp;

    //PoolAble
    public void OnSpawn()
    {
        m_bHit = false;
        m_pCollider.enabled = true;
        m_pNavMeshAgent.avoidancePriority = 40;
    }
    public void OnDespawn()
    {

    }

    public void SetPoolKey(string _strKey)
    {
        m_strPoolKey = _strKey;
    }

    virtual protected void Awake()
    {
        m_pMonsterInfo = GetComponent<ObjectInfo>();
        m_pRigidbody = GetComponent<Rigidbody>();
        m_pAnimator = GetComponent<Animator>();
        m_pNavMeshAgent = GetComponent<NavMeshAgent>();
        m_pCollider = GetComponent<Collider>();

        m_pBlackbard.Self = this;
        m_pBlackbard.Agent = m_pNavMeshAgent;
        m_pBlackbard.AnimBridge = GetComponent<AnimationBridge>();
        m_pBlackbard.ObjectInfo = m_pMonsterInfo;

        m_pBHTree = GetComponent<BehaviorTree>();
        m_pBHTree.Init(m_pBlackbard, this);

        m_pCollDownModule = new CooldownModule();
        m_pCollDownModule.Init(this);
        m_pBlackbard.CooldownModule = m_pCollDownModule;

        //objectinfo
        m_pNavMeshAgent.speed = m_pMonsterInfo.Speed;
        m_pBlackbard.HpRatio = 1.0f;
    }

    virtual protected void Start()
    {
        //나중에 네트워크로 가면 문제
        m_pBlackbard.Target = GameManager.m_Instance.Player.gameObject.transform;
    }

    virtual protected void OnDisable()
    {
        PushHpBarObjectPool();
    }
    virtual protected void OnDestroy()
    {
        PushHpBarObjectPool();
    }

   
    virtual public void MonsterUpdate()
    {
        m_pCollDownModule.UpdateCooldown();

        if (m_bHit == false)
            m_pBHTree.Evaluate();
    }
   
    virtual public void MonsterLateUpdate()
    {
        Transform pHeadTr = GetMonsterPoint(ePointType.Head);
        float y = pHeadTr.position.y + 1.0f;
        m_pMonsterHPBar.transform.position = new Vector3(transform.position.x, y, transform.position.z);

        m_pMonsterHPBar?.Billboard();
    }

    MonsterSkillInfo GetMonsterSkillInfo(int _iIdx)
    {
        return m_SOMonsterInfo.skillinfo[_iIdx];
    }

    public Transform GetMonsterPoint(ePointType _eType)
    {
        for(int i = 0; i<m_listPoint.Count; ++i)
        {
            if (m_listPoint[i].ePointType == _eType)
                return m_listPoint[i].pTrasnform;
        }

        return null;
    }

    //기본 몬스터 공격
    public void SpawnAttackObject()
    {
        m_pBlackbard.HitAnimationEvent = true;
    }

    public void ClearAttackObject()
    {
        List<MonsterAttackObject> listAttack = m_pBlackbard.SpawnObjects;
        for(int i = 0; i<listAttack.Count; ++i)
        {
            listAttack[i].PushObjectPool();
        }
        
        m_pBlackbard.SpawnObjects.Clear();
    }

    public void EndHit()
    {
        m_bHit = false;
        m_pNavMeshAgent.ResetPath();
        m_pNavMeshAgent.updateRotation = true;
        m_pBlackbard.Attacking = false;
    }
    //IHealth 구현
    public void Hit()
    {
        m_pAnimator.SetTrigger("Hit");
        m_bHit = true;

        m_pBlackbard.Attacking = false;

        m_pNavMeshAgent.ResetPath();
        m_pNavMeshAgent.updateRotation = false;
    }
    public void EndAttack()
    {
        //seq에서 1번이 통과하면 바로 2번 공격 실행 ->
        //Debug.Log($"공격끝남 {m_pBlackbard.CooldownModule.TargetIdx}");
        m_pBlackbard.Attacking = false;
        m_pAnimator.speed = 1.0f;
    }

    public void RegisterHpBar(MonsterHPBar _pHPBar)
    {
        m_pMonsterHPBar = _pHPBar;
    }

    public virtual void TakeDamage(AttackInfo _pAttackInfo)
    {

        DamageManager.m_Instance.Damaged(m_pMonsterInfo, _pAttackInfo);
        m_pBlackbard.HpRatio = (float)m_pMonsterInfo.HP / m_pMonsterInfo.MaxHp;

        if (m_pMonsterInfo.HP <= 0.0f)
        {
            Dead();
        }
        else
        {
            Hit();

            knockback(_pAttackInfo);

            if (_pAttackInfo.HitSound != null)
                SoundManager.m_Instance.PlaySfx(_pAttackInfo.HitSound, transform);
        }

        m_pMonsterHPBar?.UpdateHPBar(m_pMonsterInfo.HP, m_pMonsterInfo.MaxHp);
        ClearAttackObject();
    }
    public void Heal(int _iAmount)
    {
        m_pMonsterInfo.AddHP(_iAmount);
    }

    public bool IsDead()
    {
        return CurrentHP <= 0;
    }

    private void knockback(AttackInfo _attackInfo)
    {
        // 기존 넉백 코루틴 돌고 있으면 정지
        if (m_pKnockbackRoutine != null)
        {
            StopCoroutine(m_pKnockbackRoutine);
            m_pKnockbackRoutine = null;
        }

        Vector3 vMonsterPos = m_pRigidbody.position;
        Vector3 vDir = vMonsterPos - _attackInfo.AttackerPosition;
        vDir.y = 0.0f;

        if (vDir.sqrMagnitude < 0.0001f)
            vDir = -transform.forward;

        vDir.Normalize();

        transform.rotation = Quaternion.LookRotation(-vDir, Vector3.up);


        //시간이 지나면서 점점 멈추게 (속도 감쇠)
        m_pKnockbackRoutine = StartCoroutine(knockback_coroutine(vDir,(int)_attackInfo.Power));
    }

    private IEnumerator knockback_coroutine(Vector3 _vDir, int _iPower)
    {
        float fElapsed = 0f;

        while (m_bHit == true && fElapsed <=1.0f)
        {
            float fRevElaps = 1.0f - fElapsed;
            Vector3 vDelta = _vDir.normalized * _iPower * fRevElaps * Time.deltaTime;

            transform.position += vDelta;

            fElapsed += Time.fixedDeltaTime;

            yield return null;
        }

        m_pKnockbackRoutine = null;
    }

    protected virtual void Dead()
    {
        m_bHit = true;
        m_pCollider.enabled = false;
        m_pAnimator.speed = 1.0f;
        m_pNavMeshAgent.avoidancePriority = 99; //min
        m_pAnimator.SetTrigger("Dead");

        SODropTable pDropTable = m_SOMonsterInfo.DropTable;
        if (pDropTable == null)
            return;

        ItemDataManager.m_Instance.Drop(pDropTable, transform.position);
        MonsterManager.m_Instance.DropGold(m_SOMonsterInfo.MonsterLevel);
        QuestManager.m_Instance.UpdateQuest(eQuestType.Kill, m_SOMonsterInfo.MonsterID, 1);
    }

    public void PushObjectPool()
    {
       //애니메이션 이벤트로 등록
       ObjectPoolManager.m_Instance.PushObject(ePoolType.Stack, m_strPoolKey, gameObject);
    }

   
    public void PushHpBarObjectPool()
    {
        if (m_pMonsterHPBar != null)
        {
            MonsterManager.m_Instance.PushHpBarPoolObject(m_pMonsterHPBar.gameObject);
            m_pMonsterHPBar = null;
        }
    }

    public void MinusHP(int Damage)
    {
        m_pMonsterInfo.AddHP(Damage);
        
    }
}
