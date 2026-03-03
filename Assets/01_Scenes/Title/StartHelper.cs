using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class StartHelper : MonoBehaviour
{
    [SerializeField] private SOPortal m_pStartScene = null;
    [SerializeField] private AssetReference m_pManagerScene = null;

    public void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;

    }
    public async void Enter()
    {
        if (m_pStartScene == null || m_pManagerScene == null)
            return;


        await GameSceneManager.m_Instance.StartScene(m_pStartScene, m_pManagerScene);
    }
}
