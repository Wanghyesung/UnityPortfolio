using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//매 프레임 idx를 기억하려면 어딘가에 넣어둬야 함
//때문에 내부 클래스에 보관
// Sequence

[CreateAssetMenu(menuName = "SO/BT/Selector")]
public class SOSelector : SOComposite
{
    public override INode CreateRuntime()
    {
        var list = new List<INode>();
        for (int i = 0; i < m_listChild.Count; ++i)
        {
            list.Add(m_listChild[i].CreateRuntime());
        }

        return new SelectorRuntime(list);
    }

    private class SelectorRuntime : CompositeRuntime
    {
        int cur;
        public SelectorRuntime(List<INode> _pChild) : base(_pChild)
        {
            cur = 0;
        }
        public override INode.STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            // 캐시 인덱스부터 탐색
            for (int i = 0; i < m_pListNode.Count; ++i)
            {
                int idx = (i + cur) % m_pListNode.Count;
                var state = m_pListNode[idx].Evaluate(_pBB, _fDT);
                if (state == INode.STATE.SUCCESS)
                {
                    cur = idx;
                    return INode.STATE.SUCCESS;
                }
                if (state == INode.STATE.RUN)
                {
                    cur = idx;
                    return INode.STATE.RUN;
                }
            }

            cur = 0;
            return INode.STATE.FAILED;
        }
    }
}