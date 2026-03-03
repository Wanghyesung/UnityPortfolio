using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class DamageScreenFx : MonoBehaviour
{
    [SerializeField] private Volume m_pTargetVolume = null;
    [SerializeField] private float m_fTargetIntensity = 0.3f;
    [SerializeField] private float m_fFadeInTime = 0.05f;
    [SerializeField] private float m_fHoldTime = 0.05f;
    [SerializeField] private float m_fFadeOutTime = 0.1f;

    private Vignette m_pVignette = null;
    private Coroutine m_pDamagedCoroutine = null;

    private void Awake()
    {
        if (m_pTargetVolume != null)
            m_pTargetVolume.profile.TryGet(out m_pVignette);
    }

    public void PlayHitVignette()
    {
        if (m_pVignette == null)
            return;

        if (m_pDamagedCoroutine != null)
            StopCoroutine(m_pDamagedCoroutine);

        m_pDamagedCoroutine = StartCoroutine(PostProcessing());
    }

    private IEnumerator PostProcessing()
    {
        float fStartValue = m_pVignette.intensity.value;

        // Fade in
        float fCurTime = 0.0f;
        while (fCurTime < m_fFadeInTime)
        {
            fCurTime += Time.deltaTime;
            float t = fCurTime / m_fFadeInTime;
            m_pVignette.intensity.value = Mathf.Lerp(fStartValue, m_fTargetIntensity, t);
            yield return null;
        }

        m_pVignette.intensity.value = m_fTargetIntensity;

        // Hold
        float fCurHoldTime = 0.0f;
        if(fCurHoldTime >= m_fHoldTime)
        {
            fCurHoldTime += Time.deltaTime;
            yield return null;
        }
    
        // Fade out
        fCurTime = 0.0f;
        while (fCurTime < m_fFadeOutTime)
        {
            fCurTime += Time.deltaTime;
            float a = fCurTime / m_fFadeOutTime;
            m_pVignette.intensity.value = Mathf.Lerp(m_fTargetIntensity, 0.0f, a);
            yield return null;
        }

        m_pVignette.intensity.value = 0.0f;
        m_pDamagedCoroutine = null;
    }
}
