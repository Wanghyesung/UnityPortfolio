using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHPBar : MonoBehaviour
{
    private Image m_pHPBar = null;

    [SerializeField] private float m_fLerpTime = 0.2f;

    private Coroutine m_pHPCoroutine = null;
    private Camera m_pMainCamera = null;
    private void OnEnable()
    {
        m_pHPCoroutine = null;
        m_pHPBar.fillAmount = 1.0f;
    }

    private void Awake()
    {
        m_pMainCamera = Camera.main;
        m_pHPBar = GetComponent<Image>();
    }

    public void Billboard()
    {
        Quaternion camRot = m_pMainCamera.transform.rotation;

        Vector3 euler = camRot.eulerAngles;
        euler.z = 0.0f;

        m_pHPBar.transform.rotation = Quaternion.Euler(euler);
    }
    public void UpdateHPBar(int _iCurHP, int _iMaxHp)
    {
        if (m_pHPCoroutine != null)
            StopCoroutine(m_pHPCoroutine);

        m_pHPCoroutine = StartCoroutine(LerpHP(_iCurHP, _iMaxHp));
    }

    private IEnumerator LerpHP(int _iCurHP, int _iMaxHp)
    {
        float fTargetFill = Mathf.Clamp01((float)_iCurHP / Mathf.Max(1, _iMaxHp));
        float fStartFill = m_pHPBar.fillAmount;

        float elapsed = 0f;
        while (elapsed < m_fLerpTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / m_fLerpTime);
            m_pHPBar.fillAmount = Mathf.Lerp(fStartFill, fTargetFill, t);
            yield return null;
        }

        m_pHPBar.fillAmount = fTargetFill;
        m_pHPCoroutine = null;
    }

}
