using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Common
{
    public enum TargetType   // 대상 타입
    {
        Slef,   // 자신
        Enemy,  // 적군
        Ground, // 지점
        All,    // 전체
    }

    public enum CastType     // 시전 타입
    {
        Instant,    // 즉시   : 즉발
        Charged,    // 차징   : 스킬 게이지를 모아서 발동, 최대치에 도달하면 자동 발동, 중간에 취소 가능.
        Cast,       // 시전   : 스킬 시전 준비 시간 필요함, 캐스팅 도중 동작 불가
    }

  

}