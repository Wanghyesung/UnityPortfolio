using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//using InputFrame = InputManager.InputFrame;

public class UIRayCaster
{
    private List<GraphicRaycaster> m_listRayCast = new();
    private PointerEventData m_pPointerEventData;
    private List<RaycastResult> m_listRayResult = new();

    //씬마다 가지고 있는 이벤트 시스템
    public UIRayCaster(EventSystem _pEventSystem, IEnumerable<GraphicRaycaster> _pRay)
    {
        m_pPointerEventData = new PointerEventData(_pEventSystem);
        m_listRayCast.AddRange(_pRay);
    }

    public bool IsOverUI(Vector2 _vScreenPos)
    {
        m_listRayResult.Clear();
        m_pPointerEventData.Reset();

        m_pPointerEventData.position = _vScreenPos;

        //세밀하게 검사하기 위해서
        for (int i = 0; i < m_listRayCast.Count; ++i)
        {
            if (m_listRayCast[i].isActiveAndEnabled == false) 
                continue;
           
            m_listRayCast[i].Raycast(m_pPointerEventData, m_listRayResult);
            if (m_listRayResult.Count > 0)
                return true; // UI 위에 있음
        }
       
        return false;
    }

    public void EnableInput()
    {
        for (int i = 0; i < m_listRayCast.Count; ++i)
            m_listRayCast[i].enabled = true;
    }

    public void DisableInput()
    {
        for (int i = 0; i < m_listRayCast.Count; ++i)
            m_listRayCast[i].enabled = false;
    }


}
