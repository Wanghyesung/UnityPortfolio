using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkillBuffObject : SkillObject
{
    [SerializeField] private EffectContext m_pEffectContext = new EffectContext();

    private List<SOItemEffect> m_listSpawnEffects = null;
    private List<SOItemEffect> m_listDeSpawnEffects = null;
    private List<SOItemEffect> m_listLifeTimeEffects = null;

    private bool m_bWhileLifeApply = false;

    [SerializeField] private SOAudio m_pSkillAudio = null;
    public override void OnSpawn()
    {
        if (m_pSkillAudio != null)
            SoundManager.m_Instance.PlaySfx(m_pSkillAudio, transform);
        m_bIsSkillActive = true;
        base.OnSpawn();
    }
    public override void OnDespawn()
    {
        for (int i = 0; i < m_listDeSpawnEffects.Count; ++i)
            m_listDeSpawnEffects[i].Apply(m_pEffectContext);

        m_bIsSkillActive = false;

        m_listSpawnEffects = null;
        m_listDeSpawnEffects = null;
        m_listLifeTimeEffects = null;

        base.OnDespawn();
    }

    public override void Init()
    {
        m_pEffectContext.pOwner = gameObject;
        m_pEffectContext.pTarget = m_pOwner.gameObject;

        //아이템 효과 로직(정적)과 동일하게 계수만 context에서 동적으로 변경
        for (int i = 0; i < m_listSpawnEffects.Count; ++i)
            m_listSpawnEffects[i].Apply(m_pEffectContext);
    }
    protected override void Update()
    {
        base.Update();

        if(m_bWhileLifeApply == true)
        {
            for (int i = 0; i < m_listDeSpawnEffects.Count; ++i)
                m_listDeSpawnEffects[i].Apply(m_pEffectContext);
        }
    }
    
    public override void SetInfo(SOSKill _pSkillInfo, string _strKey)
    {
        base.SetInfo(_pSkillInfo, _strKey);

        m_listSpawnEffects = _pSkillInfo.Buff.SpawnEffects;
        m_listDeSpawnEffects = _pSkillInfo.Buff.DeSpawnEffects;
        m_listLifeTimeEffects = _pSkillInfo.Buff.LifeTimeEffects;
    }

}
