using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static System.Net.Mime.MediaTypeNames;
using eSkillType = SkillRunner.eSkillType;
using eUIType = SOEntryUI.eUIType;
using Image = UnityEngine.UI.Image;


//(사용 버튼)
public class Slot : ButtonUI
{
    protected SOEntryUI m_pSOTarget = null;
    public SOEntryUI SOTarget { get => m_pSOTarget; }

    protected IContainer m_pOwner = null;
    [SerializeField] protected ButtonUI m_pCheckUI = null;

    private Image m_pSlotIcon = null;
    protected Image m_pCheckUIImage = null;
    [SerializeField] private Image m_pIcon = null;
    protected CoolDownView m_pCoolDownView = null;

    [Header("Slot Type")]
    [SerializeField] private eUIType m_eUIType = eUIType.None;
    public eUIType eUIType { get => m_eUIType; }

    protected uint m_iUIType = 0;
    protected bool m_bSlotActive = true;
    public bool IsActiveSlot { get => m_bSlotActive; }

    [SerializeField] protected int m_iSlotIdx;
    public int SlotIdx { get => m_iSlotIdx; }

    [Header("Touch")]
    private Coroutine m_pLightCoroutine = null;
    private Color m_cOriginalColor = Color.white;                         // 원래 색
    [SerializeField] private Color m_cHighlightColor = Color.white * 2;   // 밝게 만들 색
    [SerializeField] private float m_fDuration = 0.5f;                    // 깜빡이는 전체 시간

    [SerializeField] protected SOAudio m_pUsingAudio = null;
    override protected void Awake()
    {
        base.Awake();

        m_pSlotIcon = GetComponent<Image>();
        if(m_pSlotIcon != null)
            m_cOriginalColor = m_pSlotIcon.color;

        m_pOwner = GetComponentInParent<IContainer>();

        m_iUIType = (uint)m_eUIType;

        if (m_pCheckUI != null)
        {
            m_pCheckUIImage = m_pCheckUI.GetComponent<Image>();
            m_pCheckUIImage.enabled = false;
        }
    }

    protected virtual void Start()
    {
        
    }

    private void OnDestroy()
    {
        
    }
    private void Update()
    {
      
    }

    public virtual void Init()
    {
        m_iUIType = (uint)m_eUIType;
    }
    public virtual void Bind(SOEntryUI _pSOTarget)
    {
        m_pSOTarget = _pSOTarget;


        if (m_pIcon == null)
            return;

        if(m_pSOTarget == null)
        {
            m_pIcon.enabled = false;
            m_pIcon.color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
        }
        else
        {
            m_pIcon.enabled = true;
            m_pIcon.sprite = _pSOTarget.Icon;
            m_pIcon.color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
        }

        m_pIcon.color = Color.white;
    }
    public void ActiveSlot()
    {
        if (m_pCheckUIImage == null)
            return;

        m_pCheckUIImage.enabled = true;
    }

    public void UnActiveSlot()
    {
        if (m_pCheckUIImage == null)
            return;

        m_pCheckUIImage.enabled = false;
    }

    public uint GetSlotHashCode()
    {
        return m_iUIType;
    }

    public override void OnPointerDown(PointerEventData e)
    {
        if (m_pSOTarget == null)
            return;

        base.OnPointerDown(e);
    }

    public virtual void Using()
    {
        if (m_pUsingAudio != null)
            SoundManager.m_Instance.PlaySfx(m_pUsingAudio, null);
    }

    public override void OnPointerEnter(PointerEventData e)
    {
        if(m_pSlotIcon != null)
        {
            if (m_pLightCoroutine != null)
                StopCoroutine(m_pLightCoroutine);

            m_pSlotIcon.color = m_cOriginalColor;
            m_pLightCoroutine = StartCoroutine(LightingSlot());
        }

        base.OnPointerEnter(e);
    }

    public void SetCoolDownView(CoolDownView _pCoolDownView)
    {
        m_pCoolDownView = _pCoolDownView;
    }

    public void SetCoolTime(float _fCoolTime)
    { 
        if (m_pCoolDownView == null)
            return;

        m_pCoolDownView.SetCoolTime(_fCoolTime);
    }

    public void SetUse(bool _bCanUse)
    {
        if(_bCanUse == false)
            m_pIcon.color = Color.gray;
        else
            m_pIcon.color = Color.white;
    }

    public void SetSlotIdx(int _iIdx){m_iSlotIdx = _iIdx;}  
        
    private IEnumerator LightingSlot()
    {
        float fTime = 0.0f;
        float fHalf = m_fDuration * 0.5f;

        while (fTime < m_fDuration)
        {
            fTime += Time.unscaledDeltaTime;   // UI면 보통 unscaled 많이 씀

            float fRatio;
            if (fTime <= fHalf)
                fRatio = fTime / fHalf;// 처음 절반: 원래색 -> 하이라이트
        
            else
                fRatio = 1.0f - ((fTime - fHalf) / fHalf); // 나머지 절반: 하이라이트 -> 원래색

            m_pSlotIcon.color = Color.Lerp(m_cOriginalColor, m_cHighlightColor, fRatio);

            yield return null;
        }

        // 끝나면 원래 색으로 고정
        m_pSlotIcon.color = m_cOriginalColor;
    }
}



