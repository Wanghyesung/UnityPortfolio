using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup m_pMixerGroup = null;
    [SerializeField] private eAudioChannelType m_eChannelType;
    private Slider m_pSlider = null;

    private void Awake()
    {
        m_pSlider = GetComponentInChildren<Slider>();


    }
    public void OnBgmSliderChanged()
    {
        if (m_pSlider == null)
            return;

        SoundManager.m_Instance.UpdateSound(m_pSlider, m_pMixerGroup, m_eChannelType);
    }
}
