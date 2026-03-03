using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Load/PoolEntry")]
public class SOPoolEntry : ScriptableObject
{
    public ePoolType type;                      //Stack, Global
    public AssetReferenceGameObject prefabRef;  //Addressables Asset
    public int preload = 8;                     //미리 로드할 수
    public int max = 12;                        //최대로 로드할 수 있는 수
}