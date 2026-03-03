using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//(이벤토리 슬롯)
public class SlotView : ButtonUI
{
    [SerializeField] protected Image m_pIcon = null;
    [SerializeField] protected TextMeshProUGUI m_pCountBadgeText = null;
  
    protected int m_iID = -1;
    protected int m_iSlotIdx = -1; 
    
    //내 컨테이너에서 몇번째 슬롯인지
    protected SOEntryUI m_pTargetSO = null;

    protected Container m_pContainer = null;

    public SOEntryUI SOEntryUI { get => m_pTargetSO; }
    public int SlotIdx { get => m_iSlotIdx; }


   
    public void Init(Container _pContainer)
    {
        m_pContainer = _pContainer;

        //만약 Icon이 없다면 터트리게
        if(m_pIcon == null)
             m_pIcon = GetComponent<Image>();
    }

    public void OnEnable()
    {
        if (m_pTargetSO == null)
            return;

        UpdateCount(m_pTargetSO);
    }

    public virtual void Bind(SOEntryUI _pEntryUI, int _iSlotIdx)
    {
        BindData(_pEntryUI, _iSlotIdx);

        UpdateCount(_pEntryUI);
    }

    override public void OnBeginDrag(PointerEventData e)
    {
        base.OnBeginDrag(e);
        m_pContainer.OnBeginDrag(e);
    }

    override public void OnDrag(PointerEventData e)
    {
        base.OnDrag(e);
        m_pContainer.OnDrag(e);
    }

    override public void OnEndDrag(PointerEventData e)
    {
        base.OnEndDrag(e);
        m_pContainer.OnEndDrag(e);
    }

    public override void OnPointerClick(PointerEventData e)
    {
        if (m_pContainer.IsOnSelect == false)
            return;

        if(m_pContainer != null)
        {
            m_pContainer.SetTargetSlot(this);
        }
    }

    protected void BindData(SOEntryUI _pEntryUI, int _iSlotIdx)
    {
        
        if (_pEntryUI != null)
        {
            m_pTargetSO = _pEntryUI;

            m_pIcon.sprite = _pEntryUI.Icon;
            m_pIcon.enabled = true;

            m_iID = _pEntryUI.Id;
        }
        else
        {
            m_pTargetSO = null;
            m_pIcon.enabled = false;

            m_iID = -1;
        }

        m_iSlotIdx = _iSlotIdx;
    }
    public void UpdateCount(SOEntryUI _pEntryUI)
    {
        if (m_pCountBadgeText == null)
            return;

        int iCount = m_pContainer?.GetCount(_pEntryUI) ?? 0;

        bool bShow = iCount > 1; // 1개 이하면 보통 표기 안 함
        if (bShow)
        {
            m_pCountBadgeText.enabled = true;
            m_pCountBadgeText.text = iCount.ToString();
        }
        else
            m_pCountBadgeText.enabled = false;
    }


    
}
