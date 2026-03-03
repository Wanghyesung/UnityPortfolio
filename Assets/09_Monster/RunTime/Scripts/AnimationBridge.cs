using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationInfo
{
    public AnimationInfo(string _strName, string _strParamName, bool _bOn)
    {
        Name = _strName; ParamName = _strParamName; IsOn = _bOn;
    }
    public string Name;
    public string ParamName;
    public bool   IsOn;
}

public class AnimationBridge : MonoBehaviour
{
    public Animator m_pAnimator = null;

    [SerializeField] private List<AnimationInfo> ListAttack = new List<AnimationInfo>() 
        {new AnimationInfo("Attack","Attack",false)};


    [SerializeField] private AnimationInfo Run = new AnimationInfo("Run", "Run", false);
    [SerializeField] private AnimationInfo Hit = new AnimationInfo("Hit", "Hit", false);
    [SerializeField] private AnimationInfo Dead = new AnimationInfo("Dead", "Dead", false);
    [SerializeField] private AnimationInfo Defend = new AnimationInfo("Defend", "Defend", false);

    private int m_iSpeedHash;
    private List<int> m_listAttack = new List<int>();
    private int m_iMoveHash;
    private int m_iHitHash;
    private int m_iDeadHash;

    Dictionary<string, int> m_hashNameToId = new Dictionary<string, int>();
    public void Init(Animator _pAnim)
    {
        if(m_pAnimator == null)
            m_pAnimator = _pAnim;


        for(int i = 0; i<ListAttack.Count; ++i)
        {
            int iHashID = Animator.StringToHash(ListAttack[i].Name);
            m_hashNameToId[ListAttack[i].Name] = iHashID;
            m_listAttack.Add(iHashID);
        }
      
        m_iMoveHash = Animator.StringToHash(Run.Name);
        m_hashNameToId[Run.Name] = m_iMoveHash;

        m_iHitHash = Animator.StringToHash(Hit.Name);
        m_hashNameToId[Hit.Name] = m_iHitHash;

        m_iDeadHash = Animator.StringToHash(Dead.Name);
        m_hashNameToId[Dead.Name] = m_iDeadHash;
    }


    public void SetAttack(int _iIdx, bool _bOn )
    {
        m_pAnimator.SetBool(ListAttack[_iIdx].Name, _bOn);
        //m_pAnimator.SetBool(m_listAttack[_iIdx], _bOn);
    }
    public void SetAttack(in string _strName, bool _bOn)
    {
        m_pAnimator.SetBool(_strName, _bOn);
    }
    public void SetRun()
    {
        m_pAnimator.SetTrigger(Run.ParamName);
    }
    public void SetRun(bool _bOn)
    {
        m_pAnimator.SetBool(Run.ParamName, _bOn);
    }

    public void SetDefend(bool _bOn)
    {
        m_pAnimator.SetBool(Defend.ParamName, _bOn);
    }
    public void SetHit()
    {
        m_pAnimator.SetTrigger(Hit.ParamName);
    }
    public void SetDead()
    {
        m_pAnimator.SetTrigger(Dead.ParamName);
    }


    public bool CurrentClipPlayedOnce(in string _strName , int _iLayer = 0)
    {
        //전이상태라면 false
        if (m_pAnimator.IsInTransition(_iLayer)) 
            return false;

        int iHashId = -1;
        if (m_hashNameToId.TryGetValue(_strName, out iHashId) == false)
            return false;

        var tInfo = m_pAnimator.GetCurrentAnimatorStateInfo(_iLayer);
        if (tInfo.shortNameHash == iHashId &&
            tInfo.normalizedTime >= 1.0)
            return true;
        
        return false;
    }

    public bool CurrentClipPlayedAttackOnce(int _iIdx, int _iLayer = 0)
    {
        //전이상태라면 false
        if (m_pAnimator.IsInTransition(_iLayer))
            return false;

        int iHashId = -1;
        if (m_hashNameToId.TryGetValue(ListAttack[_iIdx].Name, out iHashId) == false)
            return false;

        var tInfo = m_pAnimator.GetCurrentAnimatorStateInfo(_iLayer);
        if (tInfo.shortNameHash == iHashId &&
            tInfo.normalizedTime >= 1.0)
            return true;

        return false;
    }

}
