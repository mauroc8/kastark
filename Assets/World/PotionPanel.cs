using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionPanel : MonoBehaviour
{
    void Awake()
    {
        var amount =
            Node.Query(this, "amount text")
                .GetComponent<TMPro.TextMeshProUGUI>();

        amount.text =
            $"x{Globals.potions.Value}";

        Globals
            .potions
            .Listen(this, value =>
            {
                amount.text =
                    $"x{value}";
            });
    }
}
