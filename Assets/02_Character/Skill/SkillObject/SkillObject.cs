using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour, IPoolAble
{
    protected SkillRunner m_pOwner = null;
    public SkillRunner Owner => m_pOwner;

    protected Rigidbody m_pRigidbody = null;
    protected Collider m_pCollider = null;
    protected SOSKill m_pSkill = null;

    protected List<IChargeEvent> m_listChargeEvent = new List<IChargeEvent>();
    public List<IChargeEvent> ChargeEvents => m_listChargeEvent;

    protected bool m_bIsSkillActive = false;
    public bool IsSkillActive => m_bIsSkillActive;

    [SerializeField] protected float m_fLifeTime = 10.0f;
    protected float m_fCurLifeTime = 0.0f;

    protected uint m_iCreateCount = 0;
    protected string m_strSpawnKey = string.Empty;

    public virtual void OnSpawn()
    {
        m_iCreateCount = 1;
    }
    public virtual void OnDespawn()
    {
        m_iCreateCount = 0;
    }
    public virtual void Init()
    {

    }
    protected virtual void Awake()
    {
        m_pRigidbody = GetComponentInChildren<Rigidbody>(true);
        m_pCollider = GetComponentInChildren<Collider>(true);

        var listEvent = GetComponentsInChildren<IChargeEvent>(true);
        for (int i = 0; i < listEvent.Length; ++i)
            m_listChargeEvent.Add(listEvent[i]);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        m_fCurLifeTime += Time.deltaTime;
        if (m_fCurLifeTime >= m_fLifeTime)
        {
            //풀에 반납
            m_fCurLifeTime = 0.0f;

            PushObjectPool();
        }
    }

    public virtual void SetInfo(SOSKill _pSkillInfo, string _strKey)
    {
        m_pSkill = _pSkillInfo;

        m_fLifeTime = _pSkillInfo.Option.lifeTime;
        m_strSpawnKey = _strKey;
    }

    public void PushObjectPool()
    {
        if (m_iCreateCount == 0)
            return;

        m_iCreateCount = 0;
        ObjectPoolManager.m_Instance.PushObject(ePoolType.Global, m_strSpawnKey, gameObject);
    }


    public void SetOwner(SkillRunner _pPlayerSkillRunner) { m_pOwner = _pPlayerSkillRunner; }

    public void SetSpawnKey(string _strKey) { m_strSpawnKey = _strKey; }

}
