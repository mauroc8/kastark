using UnityEngine;

[CreateAssetMenu(menuName="Kastark/Hability")]
public class Hability : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] float _initialDamage = 5;
    [SerializeField] float _initialDifficulty = 1;
    [SerializeField] DamageType _damageType = DamageType.None;

    float _damage;
    float _difficulty;

    public float Damage => _damage;
    public float Difficulty => _difficulty;
    public DamageType DamageType => _damageType;

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

        _damage = _initialDamage;
        _difficulty = _initialDifficulty;
    }

    public static float desiredEffectiveness = 0.7549f;
    public static float effectivenessAdjustFactor = 0.2104f;

    public void Cast(CreatureController target, float unadjustedEffectiveness)
    {
        var effectiveness = Mathf.Pow(unadjustedEffectiveness, _difficulty);

        _difficulty *= // Adjust difficulty.
            (1 + (effectiveness - desiredEffectiveness) * effectivenessAdjustFactor);
        
        switch (_damageType)
        {
            case DamageType.Physical:
                target.ReceiveAttack(_damage * effectiveness);
                break;
            case DamageType.Magical:
                target.ReceiveMagic(_damage * effectiveness);
                break;
            case DamageType.Shield:
                target.ReceiveShield(_damage * effectiveness);
                break;
            case DamageType.Heal:
                target.ReceiveHeal(_damage * effectiveness);
                break;
            default:
                Debug.Log($"DamageType.{_damageType} unhandled by Hability.Cast()");
                break;
        }
    }
}
