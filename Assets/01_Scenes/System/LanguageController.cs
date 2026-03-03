using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageController : MonoBehaviour
{
    [SerializeField] private List<ButtonUI> m_listLangButton = new List<ButtonUI>();

    private List<Image> m_listImage= new List<Image>();

    private void Awake()
    {
        m_listImage = new List<Image>(m_listLangButton.Count);
      
        for (int i = 0; i < m_listLangButton.Count; ++i)
        {
            Image pImage = m_listLangButton[i].GetComponent<Image>();
            m_listImage.Add(pImage);

            int iIdx = i; // 캡처 버그 방지

            m_listLangButton[i].OnDownEvt += ()=>
            {
                OnClickLangButton(iIdx);
            };
        }
    }

    private void OnClickLangButton(int _iSelectIdx)
    {
        for (int j = 0; j < m_listLangButton.Count; ++j)
        {
            if (j == _iSelectIdx)
                m_listImage[j].color = new Color(0.575f, 0.333f, 0.035f, 1.0f);//갈색
            else
                m_listImage[j].color = Color.clear;
        }
    }
    public void SelectKorean()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("ko-KR");
    }
    public void SelectEnglish()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
    }

   
}
