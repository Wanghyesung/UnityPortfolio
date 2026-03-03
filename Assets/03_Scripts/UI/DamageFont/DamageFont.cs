using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageFont : MonoBehaviour, IPoolAble
{
    //[SerializeField] private Canvas m_pCameraCanvas;

    private Camera m_pWorldCamera = null;
    private TextMeshProUGUI m_pTextMeshProUGUI = null;
   
    private Coroutine m_pCoroutine = null;


    [SerializeField] private float m_fTargetFontSize = 13.0f; // 원하는 최종 폰트 사이즈
    [SerializeField] private float m_fGrowDuration = 1.0f; // 늘어나는 시간
    [SerializeField] private float m_fMoveSpeed = 1.0f;

    private void Awake()
    {
        m_pTextMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    public void OnSpawn()
    {
       
    }
    public void OnDespawn()
    {

    }
    public void PushObjectPool()
    {
        
    }

    private IEnumerator ScaleFontToTarget(float fTargetSize, float _fDuration)
    {
        if (m_pTextMeshProUGUI == null)
            yield break;

        float fFontSize = m_pTextMeshProUGUI.fontSize;
        float fStartSize = fFontSize;
        float fElapsed = 0f;

        // 빠르게 끝내는 경우
        if (_fDuration <= 0f)
        {
            m_pTextMeshProUGUI.fontSize = fTargetSize;
            m_pCoroutine = null;
            yield break;
        }

        while (fElapsed < _fDuration)
        {
            fElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(fElapsed / _fDuration);
            // 부드럽게 보이도록 ease-out 사용 (optional)
            float fEase = 1f - Mathf.Pow(1f - t, 2f);
            m_pTextMeshProUGUI.fontSize = Mathf.Lerp(fFontSize, fTargetSize, fEase);

            //transform.position += Vector3.up * m_fMoveSpeed * Time.deltaTime;
            yield return null;
        }

        m_pTextMeshProUGUI.fontSize = fStartSize;
        m_pCoroutine = null;
        DamageManager.m_Instance.ReturnPool(gameObject);
    }

    // 유틸: 외부에서 텍스트와 목표 크기/시간을 지정해서 시작
    public void ShowFont(int _iDamage)
    {

        if (m_pTextMeshProUGUI != null)
            m_pTextMeshProUGUI.text = $"{_iDamage}";

        if (m_pCoroutine != null)
            StopCoroutine(m_pCoroutine);

        m_pCoroutine = StartCoroutine(ScaleFontToTarget(m_fTargetFontSize, m_fGrowDuration));
    }

}
