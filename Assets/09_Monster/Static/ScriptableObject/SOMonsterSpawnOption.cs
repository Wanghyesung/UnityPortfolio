using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "SO/Monster/MonsterSpawnOption")]

public class SOMonsterSpawnOption : ScriptableObject
{
    public int SpawnCount = 0;      // -1이면 무한, 그 외는 총 스폰 횟수 제한

    public float FirstSpawnDelay = 0.0f;  // 처음 등장까지 딜레이
  
    public float SpawnTime = 10.0f;       //스폰 시간
    public float SpawnRadius = 5.0f;      // 스폰 포인트 중심에서 랜덤 반경

    public bool RandomRotation = true;    // 소환 시 랜덤 회전
    public int GroupMin = 1;              // 한 번에 나오는 최소 수
    public int GroupMax = 3;
}
