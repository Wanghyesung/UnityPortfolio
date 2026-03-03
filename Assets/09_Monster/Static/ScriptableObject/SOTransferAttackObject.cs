using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using STATE = INode.STATE;
[CreateAssetMenu(menuName = "SO/ActionNode/TransferAttackObj")]

public class SOTransferAttackObject : SONode
{
    public ePointType eMonsterPointType;
    public override INode CreateRuntime()
    {
        return new TransferAttackObjRunTime(this);
    }

    private class TransferAttackObjRunTime : INode
    {
        private ePointType m_eMonsterPoint;

        public TransferAttackObjRunTime(SOTransferAttackObject _pOwner)
        {
            m_eMonsterPoint = _pOwner.eMonsterPointType;
        }

        public STATE Evaluate(Blackboard _pBB, float _fDT)
        {
            if (_pBB.SpawnObjects.Count == 0)
                return STATE.FAILED;

            List<MonsterAttackObject> listAttack = _pBB.SpawnObjects;

            for (int i = 0; i < listAttack.Count; ++i)
            {
                
                Transform pPointTr = _pBB.Self.GetMonsterPoint(m_eMonsterPoint);
                listAttack[i].transform.localPosition = Vector3.zero;
                listAttack[i].transform.SetParent(pPointTr, false);
                }
            return STATE.SUCCESS;
        }

    }
}
