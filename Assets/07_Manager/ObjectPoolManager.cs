using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.EventSystems.EventTrigger;

public interface IPoolAble
{
    void OnSpawn();          // 상태 초기화(애니/파티클 재생 등)
    void OnDespawn();        // 정리(파티클 Stop&Clear, 물리값 리셋 등)
    void PushObjectPool();

    
}

public enum ePoolType
{
    Global,
    Stack,
    End,
}

public class ObjectPoolManager : MonoBehaviour
{
    public class PoolBucket
    {
        public SOPoolEntry entry;                                     // 설정값 참조
        public GameObject prefab;                                     // 프리팹
        public AsyncOperationHandle<GameObject> handle;               // 핸들 저장 (나중에 지우기 위해)
        public Queue<GameObject> pool = new Queue<GameObject>();      // 인스턴스 풀
    }

    private List<Dictionary<string, PoolBucket>> m_listPoolBucket;
    [SerializeField] private List<SOPoolEntry> m_listFixed = new List<SOPoolEntry>();
    [SerializeField] private List<SOPoolEntry> m_listFixedItem = new List<SOPoolEntry>();

    private List<string> m_listDeleteName = new List<string>();
    //private readonly SemaphoreSlim m_pSemaphore = new SemaphoreSlim(4, 4); // 동시 Instantiate 
    public static ObjectPoolManager m_Instance { get; private set; }

    public static bool CompletedLoad = false;
    private async void Awake()
    {
        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        m_listPoolBucket = new List<Dictionary<string, PoolBucket>>();
        for (int i = 0; i < (int)ePoolType.End; ++i)
            m_listPoolBucket.Add(new Dictionary<string, PoolBucket>());

        var listTask = new List<Task>();
        for(int i = 0; i<m_listFixed.Count; i++)
            listTask.Add(LoadObject(m_listFixed[i]));

        for (int i = 0; i < m_listFixedItem.Count; i++)
            listTask.Add(LoadObject(m_listFixedItem[i]));


        await Task.WhenAll(listTask);

        CompletedLoad = true;
    }

    public async Task<PoolBucket> LoadObject(SOPoolEntry _pPoolEntry)
    {
        var prefabRef = _pPoolEntry.prefabRef;
        if (prefabRef == null)
            return null;

        //버킷 가져오기 
        var hashPoolBucket = m_listPoolBucket[(int)_pPoolEntry.type];
        if (hashPoolBucket.TryGetValue(prefabRef.AssetGUID, out var pPool) == true)
            return pPool;

        var pBucket  = new PoolBucket();
        pBucket.entry = _pPoolEntry;

        //비동기 로드
        GameObject pPrefab = null;
        pBucket.handle = _pPoolEntry.prefabRef.LoadAssetAsync();
        pPrefab = await pBucket.handle.Task;
        pBucket.prefab = pPrefab;
        
        //로드한 프리팹을 통해 오브젝트 인스턴스
        for (int i = 0; i<_pPoolEntry.preload; ++i)
        {            
            GameObject pGameObject = GameObject.Instantiate(pPrefab);
            pGameObject.SetActive(false);
            
            //만약 타입이 글로벌이면 씬을 넘어갈 때 지우지 못하게 내 자식으로 편입
            if (_pPoolEntry.type == ePoolType.Global)
                pGameObject.transform.SetParent(gameObject.transform);
          
            pBucket.pool.Enqueue(pGameObject);

            await Task.Yield();
        }

        //프리팹 key값에 맞게 버킷 등록
        hashPoolBucket[prefabRef.AssetGUID] = pBucket;
        return pBucket;
    }

 
    public void DeleteObject(ePoolType _eType)
    {
        StartCoroutine(DeleteObjectAsync(_eType));
    }

    private IEnumerator DeleteObjectAsync(ePoolType type)
    {
        var hashPoolBucket = m_listPoolBucket[(int)type];
        if (hashPoolBucket == null)
            yield break;

        List<string> lisKey = new List<string>(hashPoolBucket.Keys);

        foreach (var key in lisKey)
        {
            var pBucket = hashPoolBucket[key];
            
            //버킷에 있는 오브젝트 정리
            while (pBucket.pool.Count > 0)
            {
                var pObj = pBucket.pool.Dequeue();
                if (pObj != null)
                    GameObject.Destroy(pObj);

                // 한 프레임에 너무 많이 Destroy하지 않게 이번 프레임 양보 
                yield return null;
            }

            Addressables.Release(pBucket.handle);
            hashPoolBucket.Remove(key);
        }
    }



    public GameObject GetObject(ePoolType _eType, string _strKey , in Vector3 _vPosition, in Vector3 _vRot)
    {
        var hashPoolBucket = m_listPoolBucket[(int)_eType];
        if (hashPoolBucket.TryGetValue(_strKey, out var pBucket) == false)
            return null;

        GameObject pObject = null;

        Vector3 vPosition = _vPosition;
        Vector3 vRot = _vRot;

        //기본값이라면 원래 프리팹 위치와 회전으로
        if (_vPosition == Vector3.zero)
            vPosition = pBucket.prefab.transform.position;
        if(_vRot == Vector3.zero)
            vRot = pBucket.prefab.transform.rotation.eulerAngles;

        //만약 큐에 오브젝트가 없다면 동기방식으로 로딩
        if (pBucket.pool.Count <= 0)
            pObject = GameObject.Instantiate(pBucket.prefab, vPosition, Quaternion.Euler(vRot));
        else
        {
            pObject = pBucket.pool.Dequeue();
            pObject.transform.SetPositionAndRotation(vPosition, Quaternion.Euler(vRot));
        }
        pObject.SetActive(true);

        if (pObject.TryGetComponent<IPoolAble>(out var IPoolCom) == true)
            IPoolCom.OnSpawn();

        return pObject;
    }

    public void PushObject(ePoolType _eType, string _strKey, GameObject _pObject)
    {
        if (_pObject.TryGetComponent<IPoolAble>(out var IPoolCom) == true)
            IPoolCom.OnDespawn();

        var hashPoolBucket = m_listPoolBucket[(int)_eType];
        if (hashPoolBucket.TryGetValue(_strKey, out var pBucket) == false)
        {
            Destroy(_pObject);
            return;
        }

        _pObject.SetActive(false);
        pBucket.pool.Enqueue(_pObject);
    }
}
