using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

public class MonsterAttackObject : MonoBehaviour, IPoolAble
{
    protected Monster m_pOwner = null;

    private Rigidbody m_pRigidbody = null;
    private Collider m_pCollider = null;

    private MonsterSkillInfo m_pMonsterSkillInfo = null;
    public MonsterSkillInfo MonsterSkillInfo => m_pMonsterSkillInfo;

    private AttackInfo m_pAttackInfo = new AttackInfo();

    private bool m_bNearAttack = false;
    private float m_fMoveSpeed = 0.0f;
 
    private Vector3 m_vDir = Vector3.zero;

    [SerializeField] private AssetReferenceGameObject m_pHitEffectRef = null;
    private GameObject m_pHitEffect = null;

    [SerializeField] private float m_fLifeTime = 10.0f;
    [SerializeField] private float m_fAttackTime = 0.0f;
    [SerializeField] private float m_fEndAttackTime = float.MaxValue;

    private bool m_bOnDestroy = false;
    private float m_fCurLifeTime = 0.0f;

    [SerializeField] private int m_iAttackCount = 1;
    private int m_iCurAttackCount = 0;

    private uint m_iCreateCount = 0;
    private string m_strSpawnKey = string.Empty;

    [SerializeField] private SOAudio m_pSKillAudio = null;
    [SerializeField] private SOAudio m_pSKillHitAudio = null;
    private int m_iSfxIdx = -1;
    public void Awake()
    {
        m_pRigidbody = GetComponent<Rigidbody>();
        m_pCollider = GetComponent<Collider>();
    }

    public void OnSpawn()
    {
        gameObject.SetActive(true);

        if (m_pSKillAudio != null)
            m_iSfxIdx = SoundManager.m_Instance.PlaySfx(m_pSKillAudio,null);
        
        m_iCreateCount = 1;
        m_iCurAttackCount = 0;
    }
    public void OnDespawn()
    {
        if (m_iSfxIdx != -1)
            SoundManager.m_Instance.StopSfx(m_iSfxIdx);

        m_iSfxIdx = -1;
        gameObject.SetActive(false);

        MonsterSkillInfo pEndSpawn = m_pMonsterSkillInfo.SpawnOption.EndFrameSpawnSkill;
        MonsterSkillSpawnOption pSpawnOption = pEndSpawn.SpawnOption;

        //재귀적으로 호출, 마지막 프레임에 소환할 오브젝트가 있다면
        if (pSpawnOption != null)
        {
            SOPoolEntry pSpawnEntry = pEndSpawn.SpawnOption.SOPoolEntry;

            if (pSpawnEntry != null)
            {
                GameObject pSpawnObj = 
                    ObjectPoolManager.m_Instance.GetObject(ePoolType.Stack, pSpawnEntry.prefabRef.AssetGUID,transform.position, Vector3.zero);

                if (pSpawnObj.TryGetComponent<MonsterAttackObject>(out var pAttackObj) == true)
                {
                    pAttackObj.SetOwner(m_pOwner);
                    pAttackObj.SetInfo(pEndSpawn);
                    pAttackObj.SetSpawnKey(pSpawnEntry.prefabRef.AssetGUID);
                }
            }
        }

        if(m_bOnDestroy == true)
            m_pOwner.ClearAttackObject();

        m_iCreateCount = 0;
    }

    public void OnEnable()
    {
        m_fCurLifeTime = 0.0f;
        m_vDir = Vector3.zero;
        if(m_pCollider != null)
            m_pCollider.enabled = false;
    }

    public void Update()
    {
        m_fCurLifeTime += Time.deltaTime;
        if(m_fCurLifeTime >=m_fLifeTime )
        {
            //풀에 반납
            m_fCurLifeTime = 0.0f;

            PushObjectPool();
        }


        if (m_pCollider != null)
        {
            if (m_fCurLifeTime >= m_fAttackTime)
                m_pCollider.enabled = true;
            else if (m_fCurLifeTime >= m_fEndAttackTime)
                m_pCollider.enabled = false;
        }
    }


    public void FixedUpdate()
    {
        if (m_pRigidbody == null)
            return;

        Vector3 vStep = m_vDir * m_fMoveSpeed * Time.fixedDeltaTime;
        m_pRigidbody.MovePosition(m_pRigidbody.position + vStep);
        
    }

    public void PushObjectPool()
    {
        if (m_iCreateCount == 0)
            return;

        m_iCreateCount = 0;
        ObjectPoolManager.m_Instance.PushObject(ePoolType.Stack, m_strSpawnKey, gameObject);
    }

    public void SetInfo(MonsterSkillInfo _pSkillInfo)
    {
        m_pMonsterSkillInfo = _pSkillInfo;
        m_fLifeTime = _pSkillInfo.SpawnOption.LifeTime;
        m_fAttackTime = _pSkillInfo.SkillOption.AttackTime;
        m_fMoveSpeed = _pSkillInfo.SkillOption.MoveSpeed;
        m_vDir = _pSkillInfo.SpawnOption.AttackDir.normalized;

        m_bNearAttack = m_fMoveSpeed > 0.0f ? false : true; 

        m_strSpawnKey = _pSkillInfo.SpawnOption.SOPoolEntry.prefabRef.AssetGUID;
        m_bOnDestroy = _pSkillInfo.SkillOption.DestroyOn;

        m_pAttackInfo.AttackVariance = _pSkillInfo.SkillOption.AttackVariance;
        m_pAttackInfo.Power = _pSkillInfo.SkillOption.AttackPower;
        m_pAttackInfo.Damage = _pSkillInfo.SkillOption.AttackDamage;
        m_pAttackInfo.Down = _pSkillInfo.SkillOption.isDown;
    }

    public void SetDir(in Vector3 _vDir)
    {
        m_vDir = _vDir;
        m_vDir.Normalize();
    }

    public void SetOwner(Monster _pMonster){m_pOwner = _pMonster;}

    public void SetSpawnKey(string _strKey){m_strSpawnKey = _strKey;}

    public void OnTriggerEnter(Collider other)
    {
        if (m_pMonsterSkillInfo == null)
            return;

        if ((m_pMonsterSkillInfo.SkillOption.TargetLayers.value & 
            (1 << other.gameObject.layer)) != 0)
        {
            if (check_attack_count() == false)
                return;
            
         

            Vector3 vAttackerPos = transform.position;
            if (m_bNearAttack == true)  
                vAttackerPos = m_pOwner.transform.position;

            m_pAttackInfo.HitPoint = vAttackerPos;
            m_pAttackInfo.HitPoint.y = 0.0f;
            m_pAttackInfo.HitSound = m_pSKillHitAudio;

            other.GetComponent<IHealth>().TakeDamage(m_pAttackInfo);

            StartEffect(other.transform.position);
            if (m_pSKillHitAudio != null)
                m_iSfxIdx = SoundManager.m_Instance.PlaySfx(m_pSKillHitAudio, transform);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        
    }

    private bool check_attack_count()
    {
        ++m_iCurAttackCount;
        if(m_iCurAttackCount > m_iAttackCount)
        {
            m_pCollider.enabled = false;
            return false;
        }

        return true;
    }

    private void StartEffect(Vector3 _vPoint)
    {

        //if (m_pHitEffectRef.editorAsset == null) 에디터 전용
        //    return;
        if (string.IsNullOrEmpty(m_pHitEffectRef.AssetGUID))
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

