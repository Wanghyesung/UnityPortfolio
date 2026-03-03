using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    static public ItemDataManager m_Instance = null;

    [SerializeField] private List<SOEntryUI> m_listItem = new();
    private Dictionary<int, SOEntryUI> m_hashItem = new();

    private void Awake()
    {  
        if (m_Instance != null)
        {
            Destroy(this);
            return;
        }

        m_Instance = this;
       
        for (int i = 0; i<m_listItem.Count; ++i)
        {
            IItemUI pItemUI = m_listItem[i] as IItemUI;
            if(pItemUI == null)
                continue;

            if (pItemUI.ItemData == null)
                continue;

            m_hashItem[pItemUI.ItemData.ItemID] = m_listItem[i];
        }
    }
   
    public SOEntryUI GetItemData(int _iID)
    {
        if (m_hashItem.TryGetValue(_iID, out var pResult))
            return pResult;

        return null;
    }

    public void Drop(SODropTable _pTable, in Vector3 _vCenterPos)
    {
        if (_pTable == null)
            return;

        for (int i = 0; i < _pTable.DropEntries.Count; ++i)
        {
            DropEntry pEntry = _pTable.DropEntries[i];

            if (pEntry.Item == null)
                continue;

            //확률 검사
            //if (Random.value > pEntry.Probability)
            //    continue;

            int iCount = Random.Range(pEntry.MinCount, pEntry.MaxCount + 1);

            for (int j = 0; j < iCount; ++j)
                spawn_item(pEntry.Item, _vCenterPos);
        }
    }




    private void spawn_item(SOItem pItem, in Vector3 _vCenterPos)
    {
        //랜덤 위치
        Vector3 vPos = _vCenterPos + new Vector3(
            Random.Range(-0.5f, 0.5f),
            0.0f,
            Random.Range(-0.5f, 0.5f));

        // 1) SOItem이 들고 있는 프리팹으로 생성
        GameObject pItemObj =
            ObjectPoolManager.m_Instance.GetObject(ePoolType.Global, pItem.ItemObject.AssetGUID, vPos, Vector3.zero);
    }

}
