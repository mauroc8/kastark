using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ContinueButton : MenuButton
{
    public Color disabledColor =
        Color.black;

    protected override void Awake()
    {
        var button =
            GetComponent<Button>();

        button.interactable = Globals.HasSavedGame();

        var tmp =
            GetComponentInChildren<TMPro.TextMeshProUGUI>();

        if (!Globals.HasSavedGame())
            tmp.color =
                disabledColor;
        else
            base.Awake();

    }
}
