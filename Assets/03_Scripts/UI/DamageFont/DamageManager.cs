using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DamageManager : MonoBehaviour
{
    [SerializeField] private Canvas m_pCameraCanvas = null;
    [SerializeField] private AssetReferenceGameObject m_pFontAssetRef = null;
    public static DamageManager m_Instance = null;
    
    private void Awake()
    {
        if (m_Instance == null)
            m_Instance = this;
        else if (m_Instance != this)
            Destroy(gameObject);
    }

    public void Damaged(ObjectInfo _pTarget, AttackInfo _attackInfo)
    {
        int iDamage = (int)Random.Range(
            _attackInfo.Damage - _attackInfo.AttackVariance,
            _attackInfo.Damage + _attackInfo.AttackVariance);
        
        int iDefense = _pTarget.Defense <=0 ? 1 : _pTarget.Defense;
        float fDefense = (float)iDefense / 3.0f;
        iDefense = fDefense <= 0.0f ? 1 : (int)fDefense;

        iDamage -= iDefense;
        iDamage = iDamage <= 0 ? 1 : iDamage;

        _pTarget.AddHP(iDamage * -1);

        ShowDamageFont(_pTarget.transform.position, iDamage);
    }

    public void PlayerDamaged(ObjectInfo _pTarget, AttackInfo _attackInfo)
    {
        Damaged(_pTarget, _attackInfo);
        CameraManager.m_Instance.DamagedPostProcess();
        HealthManager.m_Instance.UpdateHP();
    }

    public void ShowDamageFont(in Vector3 _vWorldPos, int _iDamage)
    {
        if (m_pFontAssetRef == null)
            return;

        GameObject pObj = ObjectPoolManager.m_Instance.GetObject(
            ePoolType.Global, m_pFontAssetRef.AssetGUID, _vWorldPos, Vector3.zero);

        pObj.transform.SetParent(m_pCameraCanvas.transform, true);
        pObj.transform.LookAt(Camera.main.transform);
        pObj.transform.position += transform.up * 3.0f;

        DamageFont pFont = pObj.GetComponent<DamageFont>();
        if (pFont != null)
            pFont.ShowFont(_iDamage);
    }
    public void ReturnPool(GameObject _pFont)
    {
        ObjectPoolManager.m_Instance.PushObject(ePoolType.Global, m_pFontAssetRef.AssetGUID, _pFont);
    }

}
