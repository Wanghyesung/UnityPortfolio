using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_pHealthText;
    [SerializeField] private TextMeshProUGUI m_pMPText;
    [SerializeField] private Image m_pHealthImage;
    [SerializeField] private Image m_pMPImage;
    [SerializeField] private ObjectInfo m_pPlayerStatus;

    public static HealthManager m_Instance = null;

    private Coroutine m_pUpdateHPCoroutine = null;
    private Coroutine m_pUpdateMPCoroutine = null;

    [SerializeField] private float m_fLerpTime = 0.2f;

    public void Awake()
    {
        if (m_Instance != null)
            Destroy(m_Instance);

        m_Instance = this;
    }
    public void Start()
    {
        if (m_pPlayerStatus == null)
            m_pPlayerStatus = GameManager.m_Instance.Player.GetComponent<ObjectInfo>();

        UpdateHP();
        UpdateMP();
    }

    public void UpdateHP()
    {
        float fHPRatio = 0.0f;

        if (m_pPlayerStatus.HP > 0)
            fHPRatio = (float)m_pPlayerStatus.HP / m_pPlayerStatus.MaxHp;

        // 이전에 돌던 HP Lerp가 있으면 중단
        if (m_pUpdateHPCoroutine != null)
            StopCoroutine(m_pUpdateHPCoroutine);

        // 현재 UI fillAmount에서 목표 비율까지 보간
        m_pUpdateHPCoroutine = StartCoroutine(Lerp(m_pHealthImage.fillAmount, fHPRatio, m_pPlayerStatus.HP,
            m_pHealthImage, m_pHealthText));
    }

    public void UpdateMP()
    {
        float fMPRatio = 0.0f;

        if (m_pPlayerStatus.MP > 0)
            fMPRatio = (float)m_pPlayerStatus.MP / m_pPlayerStatus.MaxMp;

        // 이전에 돌던 HP Lerp가 있으면 중단
        if (m_pUpdateMPCoroutine != null)
            StopCoroutine(m_pUpdateMPCoroutine);

        // 현재 UI fillAmount에서 목표 비율까지 보간
        m_pUpdateMPCoroutine = StartCoroutine(Lerp(m_pMPImage.fillAmount, fMPRatio, m_pPlayerStatus.MP,
            m_pMPImage, m_pMPText));
    }


    private IEnumerator Lerp(float _fCurRatio, float _fGoalRatio, float _fCurValue, Image _pImage, TextMeshProUGUI _pText)
    {
        // 바로 점프해야 할 정도로 아주 작은 차이면 그냥 세팅
        if (Mathf.Approximately(_fCurRatio, _fGoalRatio))
        {
            _pImage.fillAmount = _fGoalRatio;

            int iHpPercent = (int)(_fGoalRatio * 100.0f);
            _pText.text = $"{iHpPercent}% / {_fCurValue}";
            yield break;
        }

        float fElapsed = 0.0f;

        while (fElapsed < m_fLerpTime)
        {
            fElapsed += Time.deltaTime;
            float t = fElapsed / m_fLerpTime;
            if (t > 1.0f)
                t = 1.0f;

            float fRatio = Mathf.Lerp(_fCurRatio, _fGoalRatio, t);
            _pImage.fillAmount = fRatio;

            int iHpPercent = (int)(_fGoalRatio * 100.0f);
            _pText.text = $"{iHpPercent}% / {_fCurValue}";

            yield return null;
        }

        // 마지막으로 목표값으로 정확히 맞춰줌
        _pImage.fillAmount = _fGoalRatio;

        int iFinalPercent = (int)(_fGoalRatio * 100.0f);
        _pText.text = $"{iFinalPercent}% / {_fCurValue}";

        m_pUpdateHPCoroutine = null;
    }
}
