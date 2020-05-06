using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Globals;

public class Inventory : MonoBehaviour
{
    void Awake()
    {
        var potionPanelAmount =
            Query
                .From(this, "inventory potion-panel amount")
                .Get<TMPro.TextMeshProUGUI>();

        potions
            .Listen(this, value =>
            {
                potionPanelAmount.text =
                    $"x{value}";
            });

        potionPanelAmount.text =
            $"x{potions.Value}";

        var shieldPanelEquiped =
            Query
                .From(this, "inventory shield-panel equiped")
                .Get();

        var swordPanelEquiped =
            Query
                .From(this, "inventory sword-panel equiped")
                .Get();

        var magicPanelEquiped =
            Query
                .From(this, "inventory magic-panel equiped")
                .Get();

        swordPanelEquiped.SetActive(hasSword.Value);
        shieldPanelEquiped.SetActive(hasShield.Value);
        magicPanelEquiped.SetActive(hasMagic.Value);

        hasSword
            .Listen(this, swordPanelEquiped.SetActive);
        hasShield
            .Listen(this, shieldPanelEquiped.SetActive);
        hasMagic
            .Listen(this, magicPanelEquiped.SetActive);

        var swordPanelAlpha =
            Query
                .From(this, "inventory sword-panel")
                .Get<CanvasGroup>();

        var shieldPanelAlpha =
            Query
                .From(this, "inventory shield-panel")
                .Get<CanvasGroup>();

        var magicPanelAlpha =
            Query
                .From(this, "inventory magic-panel")
                .Get<CanvasGroup>();

        var potionPanelAlpha =
            Query
                .From(this, "inventory potion-panel")
                .Get<CanvasGroup>();

        swordPanelAlpha.alpha =
            hasSword.Value ? 1.0f : 0.5f;

        shieldPanelAlpha.alpha =
            hasShield.Value ? 1.0f : 0.5f;

        magicPanelAlpha.alpha =
            hasMagic.Value ? 1.0f : 0.5f;

        potionPanelAlpha.alpha =
            potions.Value > 0 ? 1.0f : 0.5f;

        hasSword
            .Listen(this, value =>
            {
                swordPanelAlpha.alpha =
                    value ? 1.0f : 0.5f;
            });
        hasShield
            .Listen(this, value =>
            {
                shieldPanelAlpha.alpha =
                    value ? 1.0f : 0.5f;
            });
        hasMagic
            .Listen(this, value =>
            {
                magicPanelAlpha.alpha =
                    value ? 1.0f : 0.5f;
            });
        potions
            .Listen(this, value =>
            {
                potionPanelAlpha.alpha =
                    value > 0 ? 1.0f : 0.5f;
            });
    }
}
