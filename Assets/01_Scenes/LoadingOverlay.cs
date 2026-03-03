using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingOverlay : MonoBehaviour
{
    [SerializeField] private Image m_pLoadingImage;
    private Coroutine m_pLoadingCoroutine = null;

    [SerializeField] private float m_fFillSpeed = 1.0f;
    private float m_fTargetFill = 0.0f;

    public void SetProgress(float _fValue)
    {
        if (m_pLoadingImage == null)
            return;

        m_fTargetFill = Mathf.Clamp01(_fValue);

        // 이미 코루틴 돌고 있으면 그대로 target만 바꿔서 이어서 감
        if (m_pLoadingCoroutine == null)
            m_pLoadingCoroutine = StartCoroutine(CoSmoothFill());
    }

    public void ShowLoadingImage()
    {
        gameObject.SetActive(true);
    }

    public void CompletedLoading()
    {
        gameObject.SetActive(false);
        m_pLoadingImage.fillAmount = 0.0f;
    }

    private IEnumerator CoSmoothFill()
    {
        while (true)
        {
            if (m_pLoadingImage == null)
                yield break;

            float fCurAmount = m_pLoadingImage.fillAmount;

            // 지정한 속도로 target 쪽으로 이동
            float fNextAmount = Mathf.MoveTowards(fCurAmount, m_fTargetFill, m_fFillSpeed * Time.unscaledDeltaTime
            );

            m_pLoadingImage.fillAmount = fNextAmount;

            // 거의 다 왔으면 종료
            if (Mathf.Approximately(fNextAmount, m_fTargetFill))
            {
                //// target이 바뀌지 않았다면 코루틴 종료
                //if (Mathf.Approximately(m_fTargetFill, fNextAmount))
                break;
            }

            yield return null;
        }

        m_pLoadingCoroutine = null;
    }
}
