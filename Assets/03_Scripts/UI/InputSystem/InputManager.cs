using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using ActionState = ActionMapper.ActionState;
using PointerInputState = ActionMapper.PointerInputState;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;



public class InputManager : MonoBehaviour
{
    private static int CompareByType(PointerBinding a, PointerBinding b)
    {
        return a.ID.CompareTo(b.ID);
    }

    //키보드 값과 터치 값을 맵핑하기 위해서 
    [System.Serializable]
    public enum eActionID
    {
        None,
        SkillDefault,
        Skill1,
        Skill2,
        MainSkill,
        Item,
        Move,
        End,
    }

    //디바이스 인풋
    [System.Serializable]
    public class ActionBinding
    {
        public eActionID ID = eActionID.None;
        public bool IsPressed = false;
        // UnityEvent 콜백 함수
        public PED InputFunction = null;                      
        public PED ReleaseFunction = null;
        //인스펙터에서 선택 가능
        public List<InputActionReference> listAction = new();
    }

    [System.Serializable]
    public class PointerBinding
    {
        public InputType ID = InputType.Tap;
        public PED PointerFunctions = null;
    }

    public enum InputType
    {
        Tap, Stay, Swipe, Drag, Pinch, Rotate, End
    };

    public struct tTouchEvent
    {
        public InputType eInputType;     // "tap","stay","swipe","drag","pinch","rotate" 등 이벤트 타입
        public Vector2 vDelta;           // 처음 프레임부터 이동량(드래그/스와이프 등)
        public Vector2 vDeltaDrag;       // 이전프레임과 비용 그래그 양
        public float fValue;             // 핀치 스케일 변화/회전 각도/롱프레스 지속시간 등 추가 값
        public bool bOverUI;             // 시작 시 UI 위였는지(정책: 시작 UI면 끝까지 UI)
    }
    public static InputManager m_Instance = null; 

    private UIRayCaster m_pUIRayCaster;         //UI를 눌렀는지 바탕을 눌렀는지 확인
    private TouchTracker m_pTouchTracker;       //해당 터치가 어떤 터치인지 가공
    private ActionMapper m_pActionMapper;       //가공된 데이터를 액션과 맵핑

   
    //기본 UI타치와 키보드와 맵핑된 액션
    [SerializeField] private List<ActionBinding> m_listActions = new();

    //바탕 클릭과 맵핑된 액션
    [SerializeField] private List<PointerBinding> m_listPointerActions = new();

    private readonly List<tTouchEvent> m_listTouchEvent = new(10);//프레임 버퍼 최대 10손가락
    private readonly PointerInputState m_pPointerState = new PointerInputState(); 
    private readonly ActionState m_pActionState = new ActionState();

    //UGUI 이벤트 시스템과 내 InputManager에 데이터를 동기화하기 위한 임시 데이터
    private ActionState m_pUGUIActionState = new ActionState();
    
    public PointerInputState PointerState => m_pPointerState;        
    public ActionState ActionState => m_pActionState;
    
    [SerializeField] private EventSystem m_pEventSystem;             // UGUI EventSystem 참조
    [SerializeField] private List<GraphicRaycaster> m_pUIRay;        // 상호작용 Canvas의 GraphicRaycaster 목록

    private void Awake()
    {
        if (m_Instance != null)
            Destroy(this);

        m_Instance = this;

        m_pUIRayCaster = new UIRayCaster(m_pEventSystem, m_pUIRay);
        m_pTouchTracker = new TouchTracker();
        m_pActionMapper = new ActionMapper();


        for(int i = 0; i<m_listActions.Count; ++i)
        {
            var pActionRef = m_listActions[i];
            if (pActionRef == null)
                continue;

            for(int j = 0; j< pActionRef.listAction.Count; ++j)
            {
                pActionRef.listAction[j].action.Enable();
            }
        }

        m_listPointerActions.Sort(CompareByType);
    }

    private void Update()
    {
        //이번 프레임 모든 클릭과 모든 터치를 가져온다
        var listActiveTouch = Touch.activeTouches;

        if (listActiveTouch.Count> 0)
        {
            //해당 터치가 어떤 터치였는지 가공(ex: 드래그, 스왑, 탭)
            m_pTouchTracker.UpdateTouchPhase(ref listActiveTouch, m_listTouchEvent, m_pUIRayCaster);

            // 가공된 터치를 지정된 액션과 맵핑
            m_pActionMapper.MapPointer(m_listTouchEvent, m_pPointerState, m_listPointerActions);
        }

        //이전 프레임의 액션 상태는 지우기
        m_pActionState.Clear();

        //UGUI와 연동된 액션
        m_pActionState.CopyFrom(m_pUGUIActionState);

        //UGUI나 다른 다바이스 기기와 연동된 캐릭터 인풋 액션들을 맵핑
        m_pActionMapper.MapDevice(m_pActionState, m_listActions);

    }

    public void BindUGUIButtonBoolean(eActionID _eID, bool _bValue)
    {
        m_pUGUIActionState.SetBoolean(_eID, _bValue);
    }

    public void BindUGUIButtonVector2D(eActionID _eID, in Vector2 _vValue)
    {
        m_pUGUIActionState.SetVector2D(_eID, _vValue);
    }
}
