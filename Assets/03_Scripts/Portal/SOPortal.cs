using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "SO/Portal")]
public class SOPortal : ScriptableObject
{
    public AssetReference CurrentScene;
    public AssetReference NextScene;
    public ePortalID ePortalID;
}
