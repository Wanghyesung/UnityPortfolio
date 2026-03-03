using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ShopManager : MonoBehaviour
{
    public enum eCurrency 
    { 
        Coin, 
        Cash, 
        Ticket 
    }

    static public ShopManager m_Instance = null;
    private void Awake()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(this);
            return;
        }

        m_Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private readonly Dictionary<eCurrency, long> m_hashWallet = new Dictionary<eCurrency, long>
    {
        { eCurrency.Coin, 10000 }, { eCurrency.Cash, 0 }, { eCurrency.Ticket, 0 }
    };

    //public event System.Action<eCurrency, long> OnChanged;

    public long Get(eCurrency _eCurrencyType) 
    { 
        return m_hashWallet[_eCurrencyType]; 
    }

    public bool CanSpend(eCurrency _eCurrencyType, long _lAmount) 
    { 
        return _lAmount >= 0 && m_hashWallet[_eCurrencyType] >= _lAmount; 
    }

    public bool Spend(eCurrency _eCurrencyType, long _lAmount)
    {
        if (!CanSpend(_eCurrencyType, _lAmount)) 
            return false;

        m_hashWallet[_eCurrencyType] -= _lAmount;
        return true;
    }

    public void Add(eCurrency _eCurrencyType, long _lAmount)
    {
        if (_lAmount <= 0) 
            return;
        m_hashWallet[_eCurrencyType] += _lAmount;
    }

    public void Set(eCurrency _eCurrencyType, long _lAmount)
    {
        m_hashWallet[_eCurrencyType] = System.Math.Max(0, _lAmount);
    }
}
