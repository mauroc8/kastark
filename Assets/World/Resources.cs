using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : UpdateAsStream
{

    void Awake()
    {
        var firstMatch3 = Node.Query(this, "match3-first");
        var secondMatch3 = Node.Query(this, "match3-second");

        var firstMatch3Board = firstMatch3.GetComponentInChildren<Board>();
        var secondMatch3Board = secondMatch3.GetComponentInChildren<Board>();

        var resourcesStream =
            Globals.playerResources;

        Stream.Merge(
            firstMatch3Board.earnedResources,
            secondMatch3Board.earnedResources
        )
            .Listen(this, resource =>
            {
                var resources = resourcesStream.Value;

                switch (resource)
                {
                    case Block.Plastic:
                        resources.plastic++;
                        break;

                    case Block.Wood:
                        resources.wood++;
                        break;

                    case Block.Organic:
                        resources.organic++;
                        break;

                    default:
                        return;
                }

                resourcesStream.Push(resources);
            });

        var resourcesPanel =
            Node.Query(this, "ui resources");

        resourcesPanel.SetActive(true);

        var plasticAmount =
            Node.Query(resourcesPanel, "plastic-amount")
                .GetComponent<ResourceAmountUI>();

        var woodAmount =
            Node.Query(resourcesPanel, "wood-amount")
                .GetComponent<ResourceAmountUI>();

        var organicAmount =
            Node.Query(resourcesPanel, "organic-amount")
                .GetComponent<ResourceAmountUI>();

        plasticAmount.SetResourceAmount(resourcesStream.Value.plastic);
        woodAmount.SetResourceAmount(resourcesStream.Value.wood);
        organicAmount.SetResourceAmount(resourcesStream.Value.organic);

        resourcesStream
            .Map(resources => resources.plastic)
            .Lazy()
            .Listen(this, plasticAmount.SetResourceAmount);

        resourcesStream
            .Map(resources => resources.wood)
            .Lazy()
            .Listen(this, woodAmount.SetResourceAmount);

        resourcesStream
            .Map(resources => resources.organic)
            .Lazy()
            .Listen(this, organicAmount.SetResourceAmount);


        var canvasGroup =
            resourcesPanel.GetComponent<CanvasGroup>();

        var worldScene =
            GetComponentInParent<WorldScene>();

        var playerCanMove =
            worldScene.State
                .Map(state => !state.LockPlayerControl)
                .Lazy();

        playerCanMove
            .AndThen(update.Always)
            .Get(canMove =>
            {
                canvasGroup.alpha =
                    Mathf.Lerp(
                        canvasGroup.alpha,
                        canMove ? 0.0f : 1.0f,
                        0.7f * Time.deltaTime
                    );
            });
    }
}
