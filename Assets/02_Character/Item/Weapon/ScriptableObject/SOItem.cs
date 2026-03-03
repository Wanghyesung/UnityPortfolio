using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "SO/Item/ITem")]
public class SOItem : ScriptableObject
{
    public int ItemID;

    public uint Level;

    public float cooldown = 1.0f;

    //아이템 기본 능력치 (static영역)
    [SerializeField] private Values m_tBaseValues;
    public Values BaseValues => m_tBaseValues;

    //사용 관련 함수
    public SOItemEffect[] EquippedEffects;
    public SOItemEffect[] ReleaseEffects;
    public SOItemEffect[] UsingEffects;

    //장착하거나 월드에 떨어질 때 사용될 오브젝트
    public AssetReferenceGameObject ItemObject;

}
