using Game.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static SOSkillUI;

using eSkillType = SkillRunner.eSkillType;
public class SkillSlot : Slot
{
    [SerializeField] private SOSkillUI m_pSOSkill = null;

    [Header("Skill Type")]
    [SerializeField] private eSkillType m_eSkillType = eSkillType.None;

    public SOSkillUI SOSkill { get => m_pSOSkill; }
    private SkillRunner m_pSkillRuner;

    protected override void Awake()
    {
        base.Awake();

        m_pCheckUI.OnClickEvt += select_skill_slot;

        m_iUIType |= (uint)m_eSkillType << 8;
    }

    public override void Init()
    {
        base.Init();
        m_iUIType |= (uint)m_eSkillType << (int)SOEntryUI.eUIType.Skill;
    }
    protected override void Start()
    {
     
        Player pPlayer = GameManager.m_Instance.Player;
        if (pPlayer == null)
            return;

        m_pSkillRuner = pPlayer.SkillRunner;

        if (m_pSOSkill != null)
            Bind(m_pSOSkill);
    }


    public override void Bind(SOEntryUI _pSOTarget)
    {
        base.Bind(_pSOTarget);
        if (_pSOTarget == null)
            return;

        m_pSOSkill = _pSOTarget as SOSkillUI;

        SetCoolTime(m_pSOSkill.Skill.Option.cooldown);

        bind_skill(m_pSOSkill.Skill);
    }

    public override void Using()
    {
        if (m_pSOSkill == null || m_pSOSkill.Skill == null)
            return;

        m_pSkillRuner.UseSkill(m_iSlotIdx, m_pCoolDownView);
    }
    private void select_skill_slot()
    {
        DataService.m_Instance.TryDropAndDelete(m_pOwner, SlotIdx);
    }

    private void bind_skill(SOSKill _pSkill)
    {
        if (m_pSkillRuner == null)
            return;

        m_pSkillRuner.SetSkillDefinition(m_iSlotIdx, _pSkill, m_pCoolDownView);
    }
    
}
