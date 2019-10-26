using StringLocalization;
using UnityEngine;

[CreateAssetMenu(menuName="Kastark/Hability")]
public class Hability : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] float _initialDamage = 5;
    [SerializeField] float _initialDifficulty = 1;
    [SerializeField] DamageType _damageType = DamageType.None;

    float _damage;

    public float Damage => _damage;
    public DamageType DamageType => _damageType;

    [Header("UI")]
    public string localizationKey;
    public Sprite imageSprite;
    public KeyCode hotkey;

    [Header("Controller")]
    public GameObject controller;
    
    private string _localizedName;

    public string LocalizedName => _localizedName;

    [System.NonSerialized] public int timesCast = 0;

    public void Init()
    {
        _localizedName = Localization.GetLocalizedString(localizationKey);

        _damage = _initialDamage;
    }
}
