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

        Globals
            .inventory
            .Initialized
            .Bind(this)
            .Map(inventory =>
            {
                for (int i = 0; i < 10; i++)
                {
                    switch (inventory[i])
                    {
                        case Items.Potion potion:
                            return potion.amount;
                    }
                }

                return 0;
            })
            .Get(value =>
            {
                amount.text =
                    $"x{value}";
            });
    }
}
