using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager m_Instance;

    SceneInstance? m_tCurScene = null;

    [SerializeField] private List<SOSceneLoadData> m_listSceneLoad = new List<SOSceneLoadData>();
    [SerializeField] private LoadingOverlay m_pLoadingOverlay = null;
    private readonly Dictionary<string, AsyncOperationHandle<IList<Object>>> m_hashLabelValue = new();
    private SemaphoreSlim m_pSemaphore = new SemaphoreSlim(4, 4);


    private float m_fLoadRatio = 0.0f;

    private void Awake()
    {
 
        if (m_Instance != null)
            Destroy(gameObject);

        m_Instance = this;
        DontDestroyOnLoad(gameObject);

        

        Addressables.InitializeAsync();
    }

    public async Task StartScene(SOPortal _pStartScene, AssetReference _pManagerScene)
    {
        m_pLoadingOverlay.ShowLoadingImage();

        SOSceneLoadData pSceneLoadData = null;
        for (int i = 0; i < m_listSceneLoad.Count; ++i)
        {
            if (m_listSceneLoad[i].CurrentScene.AssetGUID == _pStartScene.NextScene.AssetGUID)
            {
                pSceneLoadData = m_listSceneLoad[i];
                break;
            }
        }

        if (pSceneLoadData == null)
            return;


        var pResultHandle = pSceneLoadData.CurrentScene.LoadSceneAsync(LoadSceneMode.Single);
        while (pResultHandle.IsDone == false)
        {
            m_pLoadingOverlay.SetProgress(pResultHandle.PercentComplete * 0.5f);
            await Task.Yield();
        }

        var pResultHandleManager = _pManagerScene.LoadSceneAsync(LoadSceneMode.Additive);
        while (pResultHandleManager.IsDone == false)
        {
            m_pLoadingOverlay.SetProgress(0.5f + pResultHandleManager.PercentComplete * 0.5f);
            await Task.Yield();
        }

        
        m_tCurScene = pResultHandle.Result;

        FindPortal(_pStartScene.ePortalID);
        GraphicController.m_Instance.EnterScene();

        SoundManager.m_Instance.PlayBgm(pSceneLoadData.BGM);
        
        m_pLoadingOverlay.CompletedLoading();
    }

    public async Task LoadScene(SOPortal _pNextScenePortal, CancellationToken _tCT = default)
    {
        //로딩 화면시작
        m_pLoadingOverlay.ShowLoadingImage();

        //다음 씬 로드 데이터
        SOSceneLoadData pSceneLoadData = null;
        for (int i = 0; i < m_listSceneLoad.Count; ++i)
        {
            if (m_listSceneLoad[i].CurrentScene.AssetGUID == _pNextScenePortal.NextScene.AssetGUID)
            {
                pSceneLoadData = m_listSceneLoad[i];
                break;
            }
        }

        if (pSceneLoadData == null)
            return;

        //이전 씬 오브젝트 지우기
        await UnLoadScene();
        m_pLoadingOverlay.SetProgress(0.2f);

        //single로 한다면 자동으로 이전 씬 해제
        var pResultHandle = _pNextScenePortal.NextScene.LoadSceneAsync(LoadSceneMode.Single);
        while (pResultHandle.IsDone == false)
        {
            m_pLoadingOverlay.SetProgress(0.2f + pResultHandle.PercentComplete * 0.5f);
            await Task.Yield();
        }
        //가비지 컬렉터 강제 실행
        System.GC.Collect();
        m_tCurScene = pResultHandle.Result;
       
        //씬 전용 리소스 병렬 로딩
        List<Task> listTask = new List<Task>();
        for (int i = 0; i < pSceneLoadData.labelnames.Count; ++i)
            listTask.Add(LoadLabel(pSceneLoadData.labelnames[i], _tCT));

        for (int i = 0; i < pSceneLoadData.poolentries.Count; ++i)
            listTask.Add(ObjectPoolManager.m_Instance.LoadObject(pSceneLoadData.poolentries[i]));

        Task pLoadTask = Task.WhenAll(listTask);
        while (pLoadTask.IsCompleted == false)
        {
            int iCompletedCount = 0;
            for (int i = 0; i < listTask.Count; ++i)
            {
                if (listTask[i].IsCompleted)
                    iCompletedCount++;
            }

            float fProgress = (float)iCompletedCount / listTask.Count;
            m_pLoadingOverlay.SetProgress(0.7f + fProgress * 0.3f);

            await Task.Yield();
        }

        GraphicController.m_Instance.EnterScene();        
        FindPortal(_pNextScenePortal.ePortalID);
        SoundManager.m_Instance.PlayBgm(pSceneLoadData.BGM);

        m_pLoadingOverlay.CompletedLoading();
    }

    private async Task LoadLabel(string _strLabel, CancellationToken _tCT)
    {
        if (string.IsNullOrEmpty(_strLabel))
            return;
        if (m_hashLabelValue.ContainsKey(_strLabel))
            return;

        //동시접근 과부화 방지
        await m_pSemaphore.WaitAsync(_tCT);

        try
        {
            var tHandle = Addressables.LoadAssetsAsync<Object>(_strLabel, null);
            var listTask = await tHandle.Task; // 완료까지 논블로킹 대기
            m_hashLabelValue[_strLabel] = tHandle;
        }
        finally
        {
            m_pSemaphore.Release();
        }
    }


    public async Task UnLoadScene()
    {
        if (m_tCurScene != null)
        {
            //var tSceneInst = m_tCurScene.Value;

            //기존 씬은 Load할때 자동으로 언로드해줌
            //Addressables.UnloadSceneAsync(tSceneInst);
            m_tCurScene = null;
        }

        MonsterManager.m_Instance.ClearMonsters();

        foreach (var tHandle in m_hashLabelValue)
        {
            if (tHandle.Value.IsValid())
                Addressables.Release(tHandle.Value);
        }
        m_hashLabelValue.Clear();

        ObjectPoolManager.m_Instance.DeleteObject(ePoolType.Stack);

        await Task.Yield(); //한 프레임 양보
    }

    private void FindPortal(ePortalID _eTargetPortalID)
    {
        Portal[] arrPortal = GameObject.FindObjectsOfType<Portal>();

        for (int i = 0; i < arrPortal.Length; ++i)
        {
            if (arrPortal[i].PortalID == _eTargetPortalID)
            {
                arrPortal[i].EnterPlayer();
                return;
            }
        }

    }


    private IEnumerator LoadRoutine(AssetReference pSceneLoadData, AssetReference managerScene)
    {
        AsyncOperationHandle<SceneInstance> managerHandle =
            managerScene.LoadSceneAsync(LoadSceneMode.Additive, true);

        AsyncOperationHandle<SceneInstance> sceneHandle =
            pSceneLoadData.LoadSceneAsync(LoadSceneMode.Single, true);

        while (!managerHandle.IsDone || !sceneHandle.IsDone)
        {
            float progressManager = managerHandle.IsDone ? 1.0f : managerHandle.PercentComplete;
            float progressScene = sceneHandle.IsDone ? 1.0f : sceneHandle.PercentComplete;

            m_pLoadingOverlay.SetProgress(progressScene * 0.5f + progressManager * 0.5f);

            yield return null;
        }

        if (managerHandle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError("Manager scene load failed: " + managerHandle.OperationException);
            yield break;
        }

        if (sceneHandle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError("Main scene load failed: " + sceneHandle.OperationException);
            yield break;
        }

        Debug.Log("Scene load success");
    }
}