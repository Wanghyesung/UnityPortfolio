using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//아이템 스탯을 표시할 데이터
[Serializable]
public struct Values
{
    public int Int;
    public float Float;
    public Vector4 Vector4;
}

//동적인 영역
[Serializable]
public class EffectContext
{
    public GameObject pTarget;
    public GameObject pOwner;

    public Values Value;
}


//정적인 영역(SO 데이터) 로직만 재사용할 수 있게
public abstract class SOItemEffect : ScriptableObject
{
    public abstract void Apply(EffectContext _tEffectCnt);

}


