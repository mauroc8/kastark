using UnityEngine;

[CreateAssetMenu(menuName="Kastark/Hability")]
public class Hability : ScriptableObject
{
    [Header("Initial Stats")]
    [SerializeField] float _initialDamage = 5;
    [SerializeField] float _initialDifficulty = 1;
    
    public DamageType DamageType;

    [System.NonSerialized] public float Damage;
    [System.NonSerialized] public float Difficulty;

    [Header("UI")]
    public string localizationKey;
    public Sprite imageSprite;

    [Header("Controller")]
    public GameObject controller;
    
    private string _localizedName;
    private string _localizedDescription;
    private string _localizedTooltip;

    public string LocalizedName => _localizedName;
    public string LocalizedDescription => _localizedDescription;
    public string LocalizedTooltip => _localizedTooltip;

    public void Init()
    {
        _localizedName = Localization.GetLocalizedString(localizationKey);
        _localizedDescription = Localization.GetLocalizedString(localizationKey + "_description");
        _localizedTooltip = Localization.GetLocalizedString(localizationKey + "_tooltip");

        Debug.Log("Localized tooltip: " + _localizedTooltip);

        Damage = _initialDamage;
        Difficulty = _initialDifficulty;
    }
}
