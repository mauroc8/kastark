using UnityEngine;

[CreateAssetMenu(menuName="Kastark/Hability")]
public class Hability : ScriptableObject
{
    [Header("Hability")]
    public int Damage;
    public int Difficulty;
    public DamageType DamageType;

    [Header("UI")]
    public string Name;
    public Sprite ImageSprite;

    [Header("Controller")]
    public GameObject Controller;
    
    private string _localizedName;
    private string _localizedDescription;

    public string LocalizedName => _localizedName;
    public string LocalizedDescription => _localizedDescription;

    void OnEnable()
    {
        string[] keys = Localization.GetKeysFromHabilityName(Name);
        var nameKey = keys[0];
        var descriptionKey = keys[1];
        
        _localizedName = Localization.GetLocalizedString(nameKey);
        _localizedDescription = Localization.GetLocalizedString(descriptionKey);
    }
}
