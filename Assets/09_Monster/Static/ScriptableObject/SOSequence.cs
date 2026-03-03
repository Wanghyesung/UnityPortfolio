using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/BT/Sequence")]
public class SOSequence : SOComposite
{
    public override INode CreateRuntime()
    {
        var list = new List<INode>();
        for (int i = 0; i < m_listChild.Count; ++i)
            list.Add(m_listChild[i].CreateRuntime());

        return new SequenceRuntime(list);
    }

    private class SequenceRuntime : CompositeRuntime
    {
        int idx;
        public SequenceRuntime(List<INode> _pChild) : base(_pChild)
        {
            idx = 0;
        }
        public override INode.STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            for (int i = idx; i < m_pListNode.Count; ++i)
            {
                var state = m_pListNode[i].Evaluate(_pBB, _fDT);

                if (state == INode.STATE.FAILED)
                {
                    idx = 0;
                    return INode.STATE.FAILED;
                }
                if (state == INode.STATE.RUN)
                {
                    idx = i;
                    return INode.STATE.RUN;
                }
            }

            idx = 0;
            return INode.STATE.SUCCESS;
        }
    }
}

