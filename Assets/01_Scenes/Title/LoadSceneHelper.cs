using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneHelper : MonoBehaviour
{
    [SerializeField] private SOPortal m_pTargetScene = null;

    public async void Enter()
    {
        if (m_pTargetScene == null)
            return;
        GameManager.m_Instance.ResetPlayer();
        await GameSceneManager.m_Instance.LoadScene(m_pTargetScene);
    }
}
