using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolDownData
{
    public int ID;
    public float CurCooldown;
    public float MaxCooldown;
}

public class CooldownModule 
{
    private List<CoolDownData> m_listCooldown = new List<CoolDownData>();
    private Monster m_pOwner;
    private int m_iTargetIdx = -1;
    public int TargetIdx => m_iTargetIdx;

    public void UpdateCooldown()
    {
        for (int i = 0; i < m_listCooldown.Count; i++)
        {
            if (m_listCooldown[i].CurCooldown > m_listCooldown[i].MaxCooldown)
                continue;

            m_listCooldown[i].CurCooldown += Time.deltaTime;
        }
    }

    public void Init(Monster _pMonster)
    {
        m_pOwner = _pMonster;

        var pSkillInfo = m_pOwner.SOMonsterInfo;

        for (int i = 0; i < pSkillInfo.skillinfo.Count; ++i)
        {
            m_listCooldown.Add(new CoolDownData()
            {
                ID = i,
                CurCooldown = 0.0f,
                MaxCooldown = pSkillInfo.skillinfo[i].SkillOption.CoolTime,
            });
        }
    }

    public bool IsIsReady(int _idx)
    {
        if (_idx < 0 || _idx >= m_listCooldown.Count)
            return false;

        return m_listCooldown[_idx].CurCooldown >= m_listCooldown[_idx].MaxCooldown;
    }

    public int AnyReady()
    {
        for (int i = 0; i < m_listCooldown.Count; ++i)
        {
            if (IsIsReady(i))
            {
                return i;
            }
        }

        return -1;
    }

    public void StartCooldown(int _idx)
    {
        if (_idx < 0 || _idx >= m_listCooldown.Count)
            return;

        m_iTargetIdx = _idx;
        m_listCooldown[m_iTargetIdx].CurCooldown = 0.0f;
    }
}
