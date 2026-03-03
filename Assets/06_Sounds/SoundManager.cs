using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;
using Slider = UnityEngine.UI.Slider;

[Serializable]
public enum eAudioChannelType
{
    BGM,
    SFX,
    UI
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager m_Instance = null;

    
    [Header("BGM")]
    [SerializeField] private AudioSource m_pBgmSource;

    [Header("SFX Pool")]
    [SerializeField] private int m_iSfxPoolCount = 10;
    [SerializeField] private AudioSource m_pSfxPrefab;

    [SerializeField] private AudioMixerGroup m_pAudioMixerGroup = null; 
    private List<AudioSource> m_listSfxSource = new List<AudioSource>();
    private int m_iSfxIndex = 0;

    private void Awake()
    {
        if(m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        m_Instance = this;

        BuildSfxPool();
    }


    private void BuildSfxPool()
    {
        m_listSfxSource.Clear();

        //게임 시작 시 지정한 개수만큼 사운드소스 생성
        for (int i = 0; i < m_iSfxPoolCount; ++i)
        {
            AudioSource pSrc;
            if (m_pSfxPrefab != null)
            {
                pSrc = Instantiate(m_pSfxPrefab, transform);
            }
            else
            {
                GameObject pObj = new GameObject("SFX_" + i);
                pObj.transform.SetParent(transform);
                pSrc = pObj.AddComponent<AudioSource>();
            }

            pSrc.playOnAwake = false;
            pSrc.loop = false;
            m_listSfxSource.Add(pSrc);
        }
    }


    public void PlayBgm(SOAudio _pAudio)
    {
        if (_pAudio == null || _pAudio.Clips.Count == 0)
            return;

        AudioClip pClip = _pAudio.Clips[0];
        m_pBgmSource.outputAudioMixerGroup = _pAudio.OutputGroup;
        m_pBgmSource.clip = pClip;
        m_pBgmSource.loop = true;
        m_pBgmSource.volume = _pAudio.Volume;
        m_pBgmSource.pitch = 1.0f;
        m_pBgmSource.spatialBlend = 0.0f;
        m_pBgmSource.Play();
    }

    public void StopBgm()
    {
        m_pBgmSource.Stop();
        m_pBgmSource.clip = null;
    }


    public int PlaySfx(SOAudio _pAudio, Transform _pTransform)
    {
        if (_pAudio == null || _pAudio.Clips.Count == 0)
            return -1;

        //미리 만들어 둔 m_listSfxSource에서 하나를 고기
        int iSrcIdx = GetNextSfxSourceIdx();
        AudioSource pSrc = m_listSfxSource[iSrcIdx];

        //같은 효과음도 여러 변형 클립을 두고 랜덤 재생하면 반복감이 줄어든다
        int iClipIdx = UnityEngine.Random.Range(0, _pAudio.Clips.Count);
        pSrc.clip = _pAudio.Clips[iClipIdx];
        //믹서 그룹 연결 (BGM, SFX ..)
        pSrc.outputAudioMixerGroup = _pAudio.OutputGroup;

        //피치를 약간씩 다르게 들려서 연속 재생 피로도 감소
        pSrc.volume = _pAudio.Volume;
        pSrc.pitch = UnityEngine.Random.Range(_pAudio.PitchMin, _pAudio.PitchMax);

        //3D 위치라면 윌드 위치 기반 사운드 재생
        if (_pAudio.spatial3D == true && _pTransform != null)
        {
            pSrc.transform.position = _pTransform.position;
            pSrc.spatialBlend = _pAudio.SpatialBlend;
        }
        else
            pSrc.spatialBlend = 0.0f;

        pSrc.Play();

        return iSrcIdx;
    }
    public void StopSfx(int _iSrcIdx)
    {
        if (_iSrcIdx >= m_iSfxPoolCount || _iSrcIdx < 0)
            return;

        m_listSfxSource[_iSrcIdx].Stop();

    }

    private int GetNextSfxSourceIdx()
    {
        if (m_listSfxSource.Count == 0)
            return -1;

        int iClipIdx = m_iSfxIndex;
        ++m_iSfxIndex;
        if (m_iSfxIndex >= m_listSfxSource.Count)
            m_iSfxIndex = 0;

        return iClipIdx;
    }

 
    public void UpdateSound(Slider _pSlider, AudioMixerGroup _pGroup, eAudioChannelType _eType)
    {

        float fDB = _pSlider.value <= 0.0001f ? -80f : Mathf.Log10(_pSlider.value) * 20f;

        switch (_eType)
        {
            case eAudioChannelType.BGM:
                _pGroup.audioMixer.SetFloat("BGMVolume", fDB);
                break;

            case eAudioChannelType.SFX:
                _pGroup.audioMixer.SetFloat("SFXVolume", fDB);
                break;

            case eAudioChannelType.UI:
                _pGroup.audioMixer.SetFloat("UIVolume", fDB);
                break;
        }
    }
}
