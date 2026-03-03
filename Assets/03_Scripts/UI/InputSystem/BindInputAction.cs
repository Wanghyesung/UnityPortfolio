using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputManager;

public class BindInputAction : MonoBehaviour
{
    [SerializeField] private eActionID m_eActionID = eActionID.None;
    public eActionID ActionID { get => m_eActionID; }
    
    public void Action()
    {
        InputManager.m_Instance.BindUGUIButtonBoolean(m_eActionID, true);
    }

    public void Release()
    {
        InputManager.m_Instance.BindUGUIButtonBoolean(m_eActionID, false);
    }
}
