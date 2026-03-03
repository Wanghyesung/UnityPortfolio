using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LoadSceneHelper m_pLoadSceneHelper = null;
    [SerializeField] private Player m_pPlayer;
    public Player Player { get { return m_pPlayer; } }

    public static GameManager m_Instance = null; 

  
    private void Awake()
    {
        if (m_Instance!=null)
            Destroy(m_Instance);

        m_Instance = this;

    }

    public void LockPlayer()
    {
        if(m_pPlayer != null)
            m_pPlayer.enabled = false;   
    }

    public void UnLockPlayer()
    {
        if (m_pPlayer != null)
            m_pPlayer.enabled = true;
    }

    public void ShowReturnUI()
    {
        m_pLoadSceneHelper.gameObject.SetActive(true);
    }
 
    public void ResetPlayer()
    {
        ObjectInfo pPlayerInfo = m_pPlayer.PlayerInfo;
        pPlayerInfo.RestHP();
        pPlayerInfo.RestMP();

        HealthManager.m_Instance.UpdateHP();
        HealthManager.m_Instance.UpdateMP();

        m_pPlayer.gameObject.SetActive(true);
    }

}
