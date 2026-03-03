using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BaseUI : MonoBehaviour
{
    [SerializeField] protected bool m_bRaycast = true;
    virtual protected void Awake()
    {
        Graphic pGraphic = GetComponent<Graphic>();
        if (pGraphic == null)
            return;

        pGraphic.raycastTarget = m_bRaycast;
    }


    public void SetRaycast(bool _bRaycast)
    {
        m_bRaycast = _bRaycast;
        Graphic pGraphic = GetComponent<Graphic>();
        if (pGraphic == null)
            return;

        pGraphic.raycastTarget = m_bRaycast;
    }

}
