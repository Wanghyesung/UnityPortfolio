using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GraphicController : MonoBehaviour
{
    public static GraphicController m_Instance = null;
    private UniversalRenderPipelineAsset m_pURP = null;

    private Light[] m_arrCurLight = null;
    private bool m_bShadowOn = true;

    [SerializeField] private List<ButtonUI> m_listShadowButton = new List<ButtonUI>();
    private List<Image> m_listImage = new List<Image>();


    public void Awake()
    {
        if (m_Instance != null)
            Destroy(gameObject);

        m_Instance = this;

        RenderPipelineAsset pRenderPip = GraphicsSettings.currentRenderPipeline;
        m_pURP = pRenderPip as UniversalRenderPipelineAsset;

        //버튼 콜백함수
        m_listImage = new List<Image>(m_listShadowButton.Count);
        for (int i = 0; i < m_listShadowButton.Count; ++i)
        {
            Image pImage = m_listShadowButton[i].GetComponent<Image>();
            m_listImage.Add(pImage);

            int iIdx = i; // 캡처 버그 방지

            m_listShadowButton[i].OnDownEvt += () =>
            {
                OnClickShadowButton(iIdx);
            };
        }
    }

    private void OnClickShadowButton(int _iSelectIdx)
    {
        for (int j = 0; j < m_listImage.Count; ++j)
        {
            if (j == _iSelectIdx)
                m_listImage[j].color = new Color(0.575f, 0.333f, 0.035f, 1.0f);//갈색
            else
                m_listImage[j].color = Color.clear;
        }
    }

    //렌더스케일 조절
    public void ChangeRenderScale(float _fScale)
    {
        m_pURP.renderScale = _fScale;
    }

    //그림자 거리 조절
    public void ChangeShadowDistance(float _fScale)
    {
        m_pURP.shadowDistance = _fScale;
    }


    public void EnterScene()
    {
        //현재 씬에 모든 Light가져오기 (그림자 옵션을 위해)
        m_arrCurLight = FindObjectsOfType<Light>();
        change_shadow();
    }

    public void ChangeShadow(bool _bOn)
    {
        m_bShadowOn = _bOn;

        if (m_arrCurLight == null)
            return;

        change_shadow();
    }

    private void change_shadow()
    {
        LightShadows eLightType = m_bShadowOn == true ? LightShadows.Hard : LightShadows.None;
        for (int i = 0; i < m_arrCurLight.Length; ++i)
            m_arrCurLight[i].shadows = eLightType;
    }



}
