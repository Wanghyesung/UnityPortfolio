using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCallback : MonoBehaviour
{
    public Action m_CompletedAction = null;

    public void SetCompletedAction(Action completedAction)
    {
        m_CompletedAction = completedAction;
    }
    public void OnParticleSystemStopped()
    {
        m_CompletedAction?.Invoke();
        m_CompletedAction = null;
    }
}
