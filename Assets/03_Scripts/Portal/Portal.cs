using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//맵에 최대 6개의 문
[Serializable]
public enum ePortalID
{
    ID_1 = 0, ID_2 = 1, ID_3 = 2, ID_4 = 3, ID_5 = 4, ID_6 = 5,
}

public class Portal : MonoBehaviour
{
    [SerializeField] private LayerMask m_tPlayerLayer;
    [SerializeField] private Transform m_pSpawnPos = null;
    [SerializeField] private SOPortal m_pProtal = null;
    public SOPortal SOPortal => m_pProtal;

    [SerializeField] private ePortalID m_ePortalID = ePortalID.ID_1;
    public ePortalID PortalID => m_ePortalID;

    //나중에 SceneSO만들어서 이름 대신에 넣기
    private async void OnTriggerEnter(Collider other)
    {
        if((m_tPlayerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            await GameSceneManager.m_Instance.LoadScene(m_pProtal);   
        }
    }


    public void EnterPlayer()
    {
        Player pPlayer = GameManager.m_Instance.Player;
        pPlayer.EnterPlayer(m_pSpawnPos);
       
    }

  
}
