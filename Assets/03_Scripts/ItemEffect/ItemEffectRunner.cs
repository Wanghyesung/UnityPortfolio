using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//추가할 타입의 값
public enum EffectAdditionType
{
    None,
    Int,
    Float,
    Vector4,
}

//추가 효과 계수
public struct AdditionalEffect
{
    public EffectAdditionType Type;
    public EffectOperation Operation;
    public Values Value;
}

public class ItemEffectRunner : MonoBehaviour
{
   
    public static void ApplyEffectEquipped(SOItem _pSOItem, EffectContext _pCtx)
    {
        //foreach는 내부적으로 열거자(Enumerator) 를 만든 뒤 MoveNext()/Current로 도는 문법
        //열거자 객체가 힙에 만들어지면(참조형/박싱) → 임시 객체가 생기고 → 수집 대상이 되어 GC 스파이크
        SettingValue(_pSOItem, _pCtx);
        for (int i = 0; i< _pSOItem.EquippedEffects.Length; ++i)
            _pSOItem.EquippedEffects[i].Apply(_pCtx);
    }

    public static void ApplyEffectRelease(SOItem _pSOItem, EffectContext _pCtx)
    {
        SettingValue(_pSOItem, _pCtx);
        for (int i = 0; i < _pSOItem.ReleaseEffects.Length; ++i)
            _pSOItem.ReleaseEffects[i].Apply(_pCtx);
    }

    public static void ApplyEffectUsing(SOItem _pSOItem, EffectContext _pCtx)
    {
        //아이템 기본 스탯(정적 데이터)을 바탕으로 EffectContext(동적 데이터)값 채우기
        SettingValue(_pSOItem, _pCtx);
        //아이템 효과 로직 실행
        for (int i = 0; i < _pSOItem.UsingEffects.Length; ++i)
            _pSOItem.UsingEffects[i].Apply(_pCtx);   
    }
    


    private static void SettingValue(SOItem _pSOItem, EffectContext _pCtx)
    {
        _pCtx.Value = _pSOItem.BaseValues;
    }

};


