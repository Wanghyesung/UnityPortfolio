using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using STATE = INode.STATE;
public interface INode
{
    public enum STATE { RUN, SUCCESS, FAILED }
    public STATE Evaluate(Blackboard _pBB, float _fDT);
}

//시퀀스 노드, 셀렉트 노드를 위한 노드(자식 노드를 가질 수 있는 노드)
public abstract class CompositeRuntime : INode
{
    protected readonly List<INode> m_pListNode;
    protected CompositeRuntime(List<INode> _listChild) 
    {
        m_pListNode = _listChild;
    }
    public abstract STATE Evaluate(Blackboard _pBB, float _fDT);
}

//각 Action에 필드를 중복 보관하지 않아도 되고, 다른 노드 간 정보 전달이 쉬워진다
//기본적으로 필요한 정보 보관

[Serializable]
public class Blackboard
{
    public Monster Self;
    public Transform Target;

    public float RunTime = 0.0f;
    public float DistanceToTarget;
    public float HpRatio;

    public bool HitAnimationEvent; 
    public Vector3 AttackSpawnPos;
    public Vector3 AttackSpawnRot;
    public Vector3 AttackDir;

    public CooldownModule CooldownModule;
    public NavMeshAgent Agent;
    public AnimationBridge AnimBridge;
    public ObjectInfo ObjectInfo;

    public bool Attacking = false;
    public List<MonsterAttackObject> SpawnObjects = new List<MonsterAttackObject>();
}

