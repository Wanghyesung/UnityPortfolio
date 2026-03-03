using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BehaviorTree : MonoBehaviour
{
    [SerializeField] private SONode m_pRoot;
    private INode m_pRuntimeRoot = null;
    private Monster m_pOwner = null;

    private Blackboard m_pBlackboard = null;
    public Monster Owner => m_pOwner;

    public void Init(Blackboard _pBB, Monster _pMonster)
    {
        m_pBlackboard = _pBB;

        m_pRuntimeRoot = m_pRoot.CreateRuntime();
        m_pBlackboard.AnimBridge.Init(GetComponent<Animator>());

        m_pOwner = _pMonster;
    }
    
    public void Evaluate()
    {
        if (m_pRuntimeRoot == null)
            return;

        m_pRuntimeRoot.Evaluate(m_pBlackboard, Time.deltaTime);
    }

 
}
