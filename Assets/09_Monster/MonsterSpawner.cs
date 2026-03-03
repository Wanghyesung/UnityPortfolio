using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;


[Serializable]
public class MonsterSpawnInfo
{
    public AssetReferenceGameObject MonsterAsset;
    public SOMonsterSpawnOption SpawnOption;
    public Transform SpawnPosition;
}

public class SpawnUpdate
{
    public int SpawnCount = 0;
    public float CurrentSpawnTime = 0.0f;
}

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<MonsterSpawnInfo> m_listMonsterSpawn = 
        new List<MonsterSpawnInfo>();

    private List<SpawnUpdate> m_listSpawnUpdate = new List<SpawnUpdate>();
    private int m_iTotalSpawnCount = 0;
    private int m_iCurSpawnCount = 0;
    private void Start()
    {
        float fNowTime = Time.time;
        m_iTotalSpawnCount = 0;
        m_iCurSpawnCount = 0;

        for (int i = 0; i<m_listMonsterSpawn.Count; i++)
        {
            m_listSpawnUpdate.Add(new SpawnUpdate());
            m_iTotalSpawnCount += m_listMonsterSpawn[i].SpawnOption.SpawnCount;
        }

    }


    private void Update()
    {
        if (m_iTotalSpawnCount <= m_iCurSpawnCount)
            return;

        for (int i = 0; i < m_listMonsterSpawn.Count; ++i)
        {
            SpawnUpdate pCurSpawnUpdate = m_listSpawnUpdate[i];
            SOMonsterSpawnOption pSpawnOpt = m_listMonsterSpawn[i].SpawnOption;
            // 총 스폰 횟수 제한
            if (pCurSpawnUpdate.SpawnCount >= pSpawnOpt.SpawnCount)
                continue;

            pCurSpawnUpdate.CurrentSpawnTime += Time.deltaTime;
            if (pCurSpawnUpdate.CurrentSpawnTime < pSpawnOpt.SpawnTime)
                continue;

            Spawn(m_listMonsterSpawn[i], pCurSpawnUpdate);

            pCurSpawnUpdate.CurrentSpawnTime = 0.0f;
        }
    }


    private void Spawn(MonsterSpawnInfo _pSpawnInfo , SpawnUpdate _pSpawnUpdate)
    {
        Vector3 vBasePos = _pSpawnInfo.SpawnPosition.position;
        SOMonsterSpawnOption pSpawnOpt = _pSpawnInfo.SpawnOption;
        // 그룹 크기
        int iGroupSize = UnityEngine.Random.Range(pSpawnOpt.GroupMin, pSpawnOpt.GroupMax + 1);

        iGroupSize = Math.Min(iGroupSize, pSpawnOpt.SpawnCount - _pSpawnUpdate.SpawnCount);

        for (int i = 0; i < iGroupSize; ++i)
        {
            Vector2 vRand = UnityEngine.Random.insideUnitCircle * pSpawnOpt.SpawnRadius;
            Vector3 vSpawnPos = vBasePos + new Vector3(vRand.x, 0f, vRand.y);

            Quaternion tRot = Quaternion.identity;
            if (pSpawnOpt.RandomRotation)
                tRot = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);

            GameObject pGameObj = ObjectPoolManager.m_Instance.GetObject(ePoolType.Stack,
                _pSpawnInfo.MonsterAsset.AssetGUID,vSpawnPos,tRot.eulerAngles);

            Monster pMonster = pGameObj.GetComponent<Monster>();
            pMonster.SetPoolKey(_pSpawnInfo.MonsterAsset.AssetGUID);

            ++_pSpawnUpdate.SpawnCount;
            ++m_iCurSpawnCount;

            MonsterManager.m_Instance.RegisterMonster(pMonster);
        }
    }
}
