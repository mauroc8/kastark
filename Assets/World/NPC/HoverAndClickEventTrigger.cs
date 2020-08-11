using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverAndClickEventTrigger : MonoBehaviour
{
    public StateStream<bool> isHovering = new StateStream<bool>(false);

    public EventStream<PointerEventData> click = new EventStream<PointerEventData>();

    void Awake()
    {
        var eventTrigger = gameObject.AddComponent<EventTrigger>();

        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback
            .AddListener(data =>
            {
                isHovering.Value = true;
            });

        eventTrigger.triggers.Add(entry);


        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerExit;
        entry.callback
            .AddListener(data =>
            {
                isHovering.Value =
                    false;
            });

        eventTrigger.triggers.Add(entry);


        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback
            .AddListener(data =>
            {
                click.Push(
                    (PointerEventData)data
                );
            });

        eventTrigger.triggers.Add(entry);
    }

    void OnEnable()
    {
        isHovering.Value =
            false;
    }
}
