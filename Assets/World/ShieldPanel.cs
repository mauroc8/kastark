using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPanel : MonoBehaviour
{
    void Awake()
    {
        var cost =
            Node.Query(this, "cost");

        var equiped =
            Node.Query(this, "equiped");

        Action<bool> equipedView = value =>
        {
            cost.SetActive(!value);
            equiped.SetActive(value);
        };

        equipedView(Globals.hasShield.Value);

        Globals.hasShield.Listen(this, equipedView);
    }
}
