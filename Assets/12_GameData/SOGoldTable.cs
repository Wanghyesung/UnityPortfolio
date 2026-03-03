using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class SOGoldTable : ScriptableObject
{
    public List<GoldRange> listGoldRange = new List<GoldRange>();

    public int GetGold(int _iLevel)
    {
        //이진 탐색
        int iLeft = 0;
        int iRight = listGoldRange.Count - 1;
        int iBestIndex = -1;

        while (iLeft <= iRight)
        {
            int mid = (iLeft + iRight) / 2;
            int minLevel = listGoldRange[mid].MinLevel;

            if (minLevel <= _iLevel)
            {
                iBestIndex = mid;
                iLeft = mid + 1;
            }
            else
                iRight = mid - 1;
        }

        if (iBestIndex == -1)
            return 0;

        GoldRange range = listGoldRange[iBestIndex];
        return range.GetGold();
    }

    [System.Serializable]
    public class GoldRange
    {
        public int MinLevel;
        public int MaxLevel;


        public int MinGold = 0;
        public int MaxGold = 0;
        public int GetGold()
        {
            return Random.Range(MinGold, MaxGold + 1);
        }
    }

    public void Clear()
    {
        listGoldRange.Clear();
    }

}
