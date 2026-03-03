using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SONode : ScriptableObject
{
    public abstract INode CreateRuntime();
}


public abstract class SOComposite: SONode
{
    public List<SONode> m_listChild = new List<SONode>();
}


