using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[CreateAssetMenu(menuName = "UIData/Catalog/Entry UI", fileName = "SOEntryUI")]
public class SOEntryUI : ScriptableObject
{
    //만약 여기서 더 는다면 64비트나 128비트로
    [System.Serializable]
    public enum eUIType
    {
        None = 0,
        Skill = 8,
        Item = 16,
        Equip = 24,
    }

    [SerializeField] LocalizedString localizedName = null;  // UI이름
    [SerializeField] private int id;                        // 고정 키
    [SerializeField] protected Sprite icon;                 // 아이콘
    [SerializeField] private eUIType type;                  // UI 타입


    protected uint hashCode = (uint)eUIType.None;

    public string Name
    {
        get
        {
            return localizedName.GetLocalizedString();
        }
    }
    public int Id => id;
    public Sprite Icon => icon;
    public eUIType Type => type;

    public LocalizedString String => localizedName;
    public virtual uint GetUIHashCode()
    {
        uint iHashCode = (uint)type;
        return iHashCode;
    }

    private void OnEnable()
    {
        if (localizedName == null)
            return;

        localizedName.StringChanged += OnNameChanged;
    }

    private void OnDisable()
    {
        if (localizedName == null)
            return;

        localizedName.StringChanged -= OnNameChanged;
    }

   

    private void OnNameChanged(string value)
    {
        name = value;
    }
}