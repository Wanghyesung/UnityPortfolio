using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Profiles/Buff", fileName = "BuffProfile")]

public class BuffProfile : ScriptableObject
{
    public List<SOItemEffect> SpawnEffects = new();
    public List<SOItemEffect> DeSpawnEffects = new();
    public List<SOItemEffect> LifeTimeEffects = new();
}
