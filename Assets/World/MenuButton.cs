using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HoverAndClickEventTrigger))]
public class MenuButton : MonoBehaviour
{
    protected virtual void Awake()
    {
        var eventTrigger =
            GetComponent<HoverAndClickEventTrigger>();

        var tmp =
            GetComponentInChildren<TMPro.TextMeshProUGUI>();

        var normalColor =
            tmp.color;

        eventTrigger
            .isHovering
            .Get(hovering =>
            {
                tmp.color =
                    hovering
                        ? Color.white
                        : normalColor
                    ;
            });
    }
}
