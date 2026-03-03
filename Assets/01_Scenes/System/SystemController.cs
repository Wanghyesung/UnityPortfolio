using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SystemController : MonoBehaviour
{
    public static SystemController m_Instance = null;
    [SerializeField] private Canvas m_pSystemCanvas = null;
    [SerializeField] private Canvas m_pOverGameCanvas = null;
    [SerializeField] private List<GameObject> m_listOption = new List<GameObject>();
    private void Awake()
    {
        if (m_Instance != null)
            Destroy(gameObject);

        m_Instance = this;
        m_pSystemCanvas.sortingOrder = 1000;//가장 위에 보이게
        gameObject.SetActive(false);

        m_pOverGameCanvas.sortingOrder = 500;
        m_pOverGameCanvas.gameObject.SetActive(false);
    }
    public void VisivleOption()
    {
        gameObject.SetActive(true);
    }

    public void ClickOption(GameObject _pGameObject)
    {
        for(int i = 0; i<m_listOption.Count; ++i)
        {
            if (_pGameObject == m_listOption[i])
                m_listOption[i].SetActive(true);
            else
                m_listOption[i].SetActive(false);
        }
    }

    public void OverGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
