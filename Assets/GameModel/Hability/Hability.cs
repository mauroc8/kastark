using UnityEngine;

[CreateAssetMenu(menuName="Kastark/Hability")]
public class Hability : ScriptableObject
{
    public string Name;

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

    public string LocalizedName => _localizedName;
    public string LocalizedDescription => _localizedDescription;

    void Init()
    {
        _localizedName = Localization.GetLocalizedString(localizationKey);
        _localizedDescription = Localization.GetLocalizedString(localizationKey + "_description");

        Damage = _initialDamage;
        Difficulty = _initialDifficulty;
    }
}
