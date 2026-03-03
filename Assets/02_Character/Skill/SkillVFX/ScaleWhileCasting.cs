using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IChargeEvent
{
    void StartEvent();
    void EndEvent();

    void UpdateEvent(float _fDeltaTime);
}

public class ScaleWhileCasting : MonoBehaviour, IChargeEvent //, ChargeEvent
                                               //차지 이벤트로 만들고 차지가 끝나면 여기 End호출해주기 SO에서
{
    private float m_fCurTime = 0.0f;

    [SerializeField] private Vector3 m_vGoalScale = Vector3.one;
    [SerializeField] private Vector3 m_vStartScale = Vector3.one;

    [SerializeField] private PED m_pCompleteEvent;
    [SerializeField] private PED m_pStartEvent;

    [SerializeField] private SOAudio m_pChargeAudio = null;
    [SerializeField] private SOAudio m_pChargeEndAudio = null;
    private int m_iSrcIdx = -1;
    public void StartEvent()
    {
        m_fCurTime = 0.0f;
        transform.localScale = m_vStartScale;

        m_pStartEvent?.Invoke();

        if (m_pChargeAudio != null)
            m_iSrcIdx = SoundManager.m_Instance.PlaySfx(m_pChargeAudio, transform);
    }

    public void EndEvent()
    {
        m_pCompleteEvent?.Invoke();
        if (m_pChargeEndAudio != null)
            SoundManager.m_Instance.PlaySfx(m_pChargeEndAudio, transform);
        if (m_iSrcIdx != -1)
            SoundManager.m_Instance.StopSfx(m_iSrcIdx);
    }

    private void OnDisable()
    {
        transform.localScale = m_vStartScale;

    }
    public void UpdateEvent(float _fRatio)
    {
        transform.localScale = Vector3.Lerp(m_vStartScale, m_vGoalScale, _fRatio);
    }
}
