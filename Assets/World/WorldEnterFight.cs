using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnterFight : UpdateAsStream
{
    void Awake()
    {
        var trigger = GetComponentInChildren<TriggerEvents>();

        var aira = GameObject.Find("aira");
        var airaCollider =
            Node.Query(aira, "interaction-trigger").GetComponent<Collider>();

        var isInTrigger =
            Stream.Merge(
                trigger.TriggerEnter
                    .Filter(collider => collider == airaCollider)
                    .Map(_ => true),
                trigger.TriggerExit
                    .Map(_ => false)
            ).Lazy();



        isInTrigger
            .AndThen(inTrigger =>
                inTrigger
                    ? update.Filter(_ => Input.GetKeyDown(KeyCode.E))
                    : Stream.None<Void>()
            )
            .Get(_ =>
            {
                Debug.Log($"Enter fight");
                Cursor.lockState = CursorLockMode.None;
                WorldBattleCommunication.MoveToScene(1);
            });



        var interactText =
            Node.Query(this, "interact-text");

        isInTrigger.Get(interactText.SetActive);
    }
}
