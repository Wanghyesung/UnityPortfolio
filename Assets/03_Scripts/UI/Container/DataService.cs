using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;

public enum eContainerType
{
    Inventory = 0,
    Equipment = 1,
    Interface = 2,
    SkillTree = 3,
    Store = 4,
}

public interface IContainer
{
    public void Init();

    public void SetVisible(bool _bOn);
    public void SelectData(int _iDataIdx, int _iCategoryIdx = 0);
    public SOEntryUI GetData(int _iDataIdx, int _iCategoryIdx = 0);
    public int GetDataAmount(int _iDataIdx, int _iCategoryIdx = 0);
    public int GetDataAmount(SOEntryUI _pSoData, int _iCategoryIdx = 0);

    //데이터를 넣을 슬롯에 이미 데이터가 있는지 확인 후 있다면 기존 슬롯 데이터 정보를 반환
    public bool AddData(int _iDataIdx, SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0);
    //비어있는칸으로 넣기
    public bool AddData(SOEntryUI _pSOData, int _iAmount, int _iCategoryIdx = 0);
    public bool DeleteData(int _iDataIdx, int _iCategoryIdx = 0);

    public bool DeleteData(SOEntryUI _pSOData, int _iAmount);
    public bool FindData(SOEntryUI _pData, int _iCategoryIdx = 0);
    public bool FindData(int _iDataIdx, int _iCategoryIdx = 0);

    public bool Consume(int _iDataIdx, int _iAmount, int _iCategoryIdx = 0);

    //무조건 타입을 가질 수 있도록
    public eContainerType ContainerType { get; }
}

public struct SlotRef
{
    public IContainer Container; //보내려는 컨테이너
    public SOEntryUI Data; //보낼 데이터
    public int CategoryIdx; //보낼 데이터의 카테고리
    public int DataIdx; //보낼 컨테이너 데이터의 인덱스
    public int Amount; //개수
}

public class DataService : MonoBehaviour
{
    //1. 모든 데이터 관리(스킬, 아이템 장비..)
    //2. 데이터 이동(인벤->장비, 인벤->상점, 인벤->버리기)
    //3. 데이터 검색(아이템, 스킬)

    [SerializeField] private List<BaseUI> m_listContainerObj;
    private List<IContainer> m_listContainer = new List<IContainer>();

    [SerializeField] private GameObject m_pPlayerInfo = null;
    //아이엠 등록 (null가능 유니티 인스펙터에서 확인 불가)
    private SlotRef? m_pTargetSlot = null;

    [SerializeField] private List<SOEquipUI> m_listPlayerStartEquip = new();

    //public void RegisterData()
    //등동된 데이터 Container로 가져오기
    //public void BringData()

    public static DataService m_Instance { get; private set; }

    private static int CompareByType(IContainer a, IContainer b)
    {
        return a.ContainerType.CompareTo(b.ContainerType);
    }

    private void Awake()
    {
      
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 최초 인스턴스 등록
        m_Instance = this;
        DontDestroyOnLoad(gameObject); // 선택: 씬 전환에도 유지  
    }

    private void Start()
    {
        Init();
        //정적 메서드 + 캐시된 델리게이트: 1회만 할당,
        m_listContainer.Sort(CompareByType);

        for (int i = 0; i < m_listContainer.Count; ++i)
            m_listContainer[i].Init();

        //플레이더 데이터 미리 셋팅
        StartCoroutine(waitPoolAndEquiped());

    }

    private void Init()
    {
        m_listContainer.Clear();

        for (int i = 0; i < m_listContainerObj.Count; ++i)
        {
            if (m_listContainerObj[i] == null)
                continue;

            IContainer IContain = m_listContainerObj[i].GetComponent<IContainer>();
            if (IContain == null)
            {
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(m_listContainerObj[i]);
#else
                Destroy(m_listContainerObj[i]);
#endif
            }
            else
                m_listContainer.Add(IContain);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Init();
    }
#endif

   
    public bool StartPickData(IContainer _pFrom, SOEntryUI _pEntryUI, int _iFromIdx, int _iAmount, int _iCategoryIdx = 0)
    {
        if (_pEntryUI == null)
            return false;

        m_pTargetSlot = new SlotRef
        {
            Container = _pFrom,
            Data = _pEntryUI,
            CategoryIdx = _iCategoryIdx,
            DataIdx = _iFromIdx,
            Amount = _iAmount
        };
        
        return true;
    }

    //아이템 오브젝트로 들어갈 때
    public bool TryDropDataObject()
    {
        return true;
    }

    //빈칸 자동으로 넣기
    public bool TryAddData(eContainerType _eToType, int _iToCategoryIdx = 0)
    {
        IContainer pTo = GetContainer(_eToType);

        SlotRef pTargetData = m_pTargetSlot.Value;
        if (pTargetData.Data == null)
            return false;

        return pTo.AddData(pTargetData.Data, pTargetData.Amount, _iToCategoryIdx);
    }

    public bool TryAddData(eContainerType _eToType, SOEntryUI _pEntryUI, int _iAmount)
    {
        IContainer pTo = GetContainer(_eToType);

        return pTo.AddData(_pEntryUI, _iAmount);
    }

    //요청한 인덱스에 넣기
    public bool TryDropData(eContainerType _eToType, int _iToIdx, int _iToCategoryIdx = 0)
    {
        IContainer pTo = GetContainer(_eToType);
        if (pTo == null)
            return false;

        return TryDropData(pTo, _iToIdx);
    }
    public bool TryDropAndDelete(IContainer _pTo, int _iToIdx)
    {
        _pTo.DeleteData(_iToIdx);
        return TryDropData(_pTo, _iToIdx);
    }
    public bool TryDropData(IContainer _pTo, int _iToIdx)
    {
        
        if (m_pTargetSlot == null)
            return false;

        SlotRef pTargetData = m_pTargetSlot.Value;
        if (_pTo.AddData(_iToIdx, pTargetData.Data, pTargetData.Amount, pTargetData.CategoryIdx) == false)
        {
            m_pTargetSlot = null;
            return false;
        }

        m_pTargetSlot.Value.Container.DeleteData(m_pTargetSlot.Value.DataIdx);
        m_pTargetSlot = null;

        return true;
    }

  
    private IContainer GetContainer(eContainerType _eType)
    {
        return m_listContainer[(int)_eType];
    }

    public bool TryDropDataAndSwap(eContainerType _eToType, int _iToIdx, int _iToCategoryIdx = 0)
    {
        IContainer pTo = GetContainer(_eToType);
        if(pTo == null)
            return false;

        return TryDropDataAndSwap(pTo, _iToIdx, _iToCategoryIdx);
    }
    public bool TryDropDataAndSwap(IContainer _pTo, int _iToIdx, int _iToCategoryIdx = 0)
    {
        if (m_pTargetSlot == null)
            return false;

        //데이터를 넣기 전에 해당 인덱스에 이미 데이터가 있다면 보관
        SOEntryUI pEntryUI = _pTo.GetData(_iToIdx);
        int iAmount = 1;

        if (pEntryUI != null)
        {
            //해당 데이터 미리 보관 후 삭제
            iAmount = _pTo.GetDataAmount(_iToIdx);
            _pTo.DeleteData(_iToIdx, _iToCategoryIdx);
        }

        SlotRef pTargetData = m_pTargetSlot.Value;
        //어떤 컨테이너에서 지정한 인덱스(상대 컨테이너)로 어떤 데이터를 얼마만큼 보낼지
        if (_pTo.AddData(_iToIdx, pTargetData.Data, pTargetData.Amount, _iToCategoryIdx) == false)
        {
            m_pTargetSlot = null;
            return false;
        }

        pTargetData.Container.DeleteData(pTargetData.DataIdx, pTargetData.CategoryIdx);

        //기존 데이터 넘겨주기
        if (pEntryUI != null)
        {
            pTargetData.Container.AddData(pTargetData.DataIdx, pEntryUI, iAmount, pTargetData.CategoryIdx);
        }

        m_pTargetSlot = null;
        return true;
    }


    public bool DeleteData(eContainerType _eToType, SOEntryUI _pData, int _iAmount)
    {
        IContainer pTo = GetContainer(_eToType);
        if (pTo == null)
            return false;

        return pTo.DeleteData(_pData, _iAmount);
    }

    private void clear_target()
    {
        m_pTargetSlot.Value.Container.DeleteData(m_pTargetSlot.Value.DataIdx);
        m_pTargetSlot = null;
    }

    public void SetVisiblePlayerInfo(bool _bEnable)
    {
        m_pPlayerInfo.SetActive(_bEnable);
    }
    public void SetVisibleContainer(eContainerType _eType, bool _bEnable)
    {
        IContainer IContain = GetContainer(_eType);
        IContain.SetVisible(_bEnable);
    }




    //처음 캐릭터 장비를 셋팅할 때 오브젝트 풀에서(비동기) 무기 오브젝트가 전부 로드되지 않을 수 있음
    private IEnumerator waitPoolAndEquiped()
    {
        yield return new WaitUntil(() => ObjectPoolManager.m_Instance != null);
        yield return new WaitUntil(() => ObjectPoolManager.CompletedLoad == true);

        for (int i = 0; i < m_listPlayerStartEquip.Count; ++i)
        {
            SOEquipUI pEquip = m_listPlayerStartEquip[i];
            m_listContainer[(int)eContainerType.Equipment].AddData(pEquip, 1);
        }
    }
}
