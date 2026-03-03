using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUI : BaseUI
{
    [SerializeField] protected SOEntryUI m_pEntryUI = null;
    virtual public SOEntryUI GetEntryUI() { return m_pEntryUI; }
}
