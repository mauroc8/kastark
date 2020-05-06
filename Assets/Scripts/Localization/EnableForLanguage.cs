using UnityEngine;

public class EnableForLanguage : MonoBehaviour
{
    public LanguageId language;

    void Awake()
    {
        gameObject.SetActive(Localization.currentLanguage == language);
    }
}
