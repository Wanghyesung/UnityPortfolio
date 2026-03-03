using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MonsterManager : MonoBehaviour
{
    //몬스터 스폰과 , 몬스터 움직임 업데이트 관리

    public static MonsterManager m_Instance = null;
    private List<Monster> m_pMonsters = new List<Monster>();
    [SerializeField] private Camera m_pMainCamera = null; //컬리전용

    [SerializeField] private AssetReferenceGameObject m_pHpBarRef = null;
    [SerializeField] private Canvas m_pWorldCanvas = null; //반환받은 UI를 어느 컨버스에 붙일지
    [SerializeField] private SOGoldTable m_pGoldTable = null;
    private void Awake()
    {

        if(m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(gameObject);

    }

    public void Update()
    {
        for (int i = 0; i < m_pMonsters.Count; ++i)
        {
            Monster pCurMonster = m_pMonsters[i];
            if (pCurMonster == null)
                continue;

            if (CameraCulling(pCurMonster.transform) == true && pCurMonster.isActiveAndEnabled == true)
                pCurMonster.MonsterUpdate();
        }
    }

    public void LateUpdate()
    {
        for (int i = 0; i < m_pMonsters.Count; ++i)
        {
            Monster pCurMonster = m_pMonsters[i];
            if (pCurMonster == null)
                continue;

            if(CameraCulling(pCurMonster.transform) == true && pCurMonster.isActiveAndEnabled == true)
                pCurMonster.MonsterLateUpdate();
        }
    }


    private bool CameraCulling(Transform _pTargetTr)
    {
        Vector3 vp = m_pMainCamera.WorldToViewportPoint(_pTargetTr.position);

        //카메라 안에 들어와있다면 업데이트
        //vp.z > 0f &&
        bool bUpdate = vp.x >= 0f && vp.x <= 1f &&
                       vp.y >= 0f && vp.y <= 1f;

        return bUpdate;
    }

    public void PushHpBarPoolObject(GameObject _pHpBar)
    {
        ObjectPoolManager.m_Instance.PushObject(ePoolType.Global, m_pHpBarRef.AssetGUID, _pHpBar);
    }

    public void RegisterMonster(Monster _pMonster)
    {
        m_pMonsters.Add(_pMonster);

        GameObject pHpBar = ObjectPoolManager.m_Instance.GetObject(ePoolType.Global, m_pHpBarRef.AssetGUID, Vector3.zero, Vector3.zero);
        pHpBar.transform.SetParent(m_pWorldCanvas.transform);

        MonsterHPBar pMonsterHpBar = pHpBar.GetComponent<MonsterHPBar>();
        _pMonster.RegisterHpBar(pMonsterHpBar);
    }

    public void ClearMonsters()
    {
        m_pMonsters.Clear();
    }


    public void DropGold(int _iMonsterLevel)
    {
        int iGold = m_pGoldTable.GetGold(_iMonsterLevel);
        ShopManager.m_Instance.Add(ShopManager.eCurrency.Coin, iGold);
    }
}
