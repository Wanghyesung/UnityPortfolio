using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class DropEntry
{
    public SOItem Item;         
    public float Probability;    
    public int MinCount = 1;
    public int MaxCount = 1;
}

[CreateAssetMenu(menuName = "SO/Monster/DropTable")]
public class SODropTable : ScriptableObject
{
    public List<DropEntry> DropEntries = new List<DropEntry>();
}
