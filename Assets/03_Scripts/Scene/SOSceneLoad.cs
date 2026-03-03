using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(menuName = "Load/SceneLoad")]
public class SOSceneLoadData : ScriptableObject
{
    public SOAudio BGM;                        //Scene 배경음
    public AssetReference CurrentScene;        //Scene
    public List<string> labelnames;            //씬에 필요한 Addressable 라벨 이름
    public List<SOPoolEntry> poolentries;      //이번 씬에 풀링할 오브젝트들
    public List<AssetReference> extraassets;   //사운드, 메테리얼 등,,
}