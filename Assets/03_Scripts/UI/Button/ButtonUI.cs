using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using eActionID = InputManager.eActionID;

// Inspector에서 PointerEventData를 넘길 수 있게 하는 UnityEvent
[Serializable] public class PED : UnityEvent { }


public class ButtonUI : BaseUI,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, 
    IPointerClickHandler
{
    //UGUI 포인터 이벤트는 Monobehaviour update전에 이벤트 발생

    //만약 InputManager와 병합한다면
   
    [SerializeField] protected BindInputAction m_pActionBind = null;

    public TextMeshProUGUI m_pTextMeshProUGUI;

    //코드 바인딩용 델리게이트
    public event Action OnEnterEvt;
    public event Action OnExitEvt;
    public event Action OnDownEvt;
    public event Action OnUpEvt;
    public event Action OnBeginDragEvt;
    public event Action OnDragEvt;
    public event Action OnEndDragEvt;
    public event Action OnClickEvt;

    // 인스펙터 바인딩용 
    [SerializeField] private PED onEnter;
    [SerializeField] private PED onExit;
    [SerializeField] private PED onDown;
    [SerializeField] private PED onUp;
    [SerializeField] private PED onBeginDrag;
    [SerializeField] private PED onDrag;
    [SerializeField] private PED onEndDrag;
    [SerializeField] private PED onClick;

    [SerializeField] private SOAudio m_pClickAudio;
    [SerializeField] private SOAudio m_pDownAudio;
    protected override void Awake()
    {
        base.Awake();


        m_pTextMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        m_pActionBind = GetComponent<BindInputAction>();
    }
   
    virtual public void OnPointerEnter(PointerEventData e)
    {
        onEnter?.Invoke();
        OnEnterEvt?.Invoke();
    }

    virtual public void OnPointerExit(PointerEventData e)
    {
        onExit?.Invoke();
        OnExitEvt?.Invoke();
    }

    virtual public void OnPointerDown(PointerEventData e)
    {
        if (m_pDownAudio != null)
            SoundManager.m_Instance.PlaySfx(m_pDownAudio,null);
        onDown?.Invoke();
        OnDownEvt?.Invoke();

        //인풋매니저에게 바인딩된 액션이 눌렸다고 알리기
        if (m_pActionBind != null)
            m_pActionBind.Action();
    }

    virtual public void OnPointerUp(PointerEventData e)
    {
        onUp?.Invoke();
        OnUpEvt?.Invoke();

        if (m_pActionBind != null)
            m_pActionBind.Release();
    }

    virtual public void OnBeginDrag(PointerEventData e)
    {
        onBeginDrag?.Invoke();
        OnBeginDragEvt?.Invoke();
    }

    virtual public void OnDrag(PointerEventData e)
    {
        onDrag?.Invoke();
        OnDragEvt?.Invoke();
    }

    virtual public void OnEndDrag(PointerEventData e)
    {
        onEndDrag?.Invoke();
        OnEndDragEvt?.Invoke();
    }
    virtual public void OnPointerClick(PointerEventData e)
    {
        if (m_pClickAudio != null)
            SoundManager.m_Instance.PlaySfx(m_pClickAudio, null);
        onClick?.Invoke();
        OnClickEvt?.Invoke();
    }
}
