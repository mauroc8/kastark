using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastSpell : MonoBehaviour
{
    public string spell = "kastark";
    public string userInput = "";

    private Text _text;

    public bool paused = false;
    public float matchRate = 0.0f;

    void Start()
    {
        _text = GetComponentInChildren<Text>();
        spell = spell.ToLower(); // TEMP now we only allow lowercase spell names.
        _text.text = spell;
    }

    void Update()
    {
        if (paused) return;

        var richText = "";

        for (int i = 0; i < spell.Length; i++)
        {
            if (i >= userInput.Length)
            {
                richText += spell[i];
            } else if (userInput[i] == spell[i])
            {
                richText += $"<color=#00ffff>{userInput[i]}</color>";
            } else {
                richText += $"<color=red>{userInput[i]}</color>";
            }
        }

        _text.text = richText;


        if (spell.Length <= userInput.Length)
        {
            paused = true;
            return;
        }

        foreach (char letter in Input.inputString)
        {
            if (letter == '\b' || letter == '\n') break;
            userInput += letter.ToString().ToLower();
        }
    }
}
