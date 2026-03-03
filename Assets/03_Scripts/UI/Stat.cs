using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class StatInfo
{
    public eObjectStat eObjectStat;
    public TextMeshProUGUI StatText;
}
public class Stat : MonoBehaviour
{
    [SerializeField] private List<StatInfo> m_listStat = new List<StatInfo>();

    private void Awake()
    {
        m_listStat.Sort((a, b) =>
        {
            return a.eObjectStat.CompareTo(b.eObjectStat);
        });
    }
    private void OnEnable()
    {
        UpdateStatText();
    }

    public void UpdateStatText()
    {
        ObjectInfo pPlayerInfo = GameManager.m_Instance.Player.PlayerInfo;
        for (int i = 0; i < m_listStat.Count; ++i)
        {
            StatInfo iStatInfo = m_listStat[i];

            int iStatValue = pPlayerInfo.GetStatValue(iStatInfo.eObjectStat);
            iStatInfo.StatText.text = iStatValue.ToString();
        }
    }

}
