using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : UpdateAsStream
{

    void Awake()
    {
        var resourcesPanel =
            Query
                .From(this, "ui resources")
                .Get();

        resourcesPanel.SetActive(true);

        var plasticAmount =
            Query
                .From(resourcesPanel, "plastic")
                .Get<ResourceAmountUI>();

        var woodAmount =
            Query
                .From(resourcesPanel, "wood")
                .Get<ResourceAmountUI>();

        var organicAmount =
            Query
                .From(resourcesPanel, "organic")
                .Get<ResourceAmountUI>();

        Globals.playerResources
            .Initialized
            .Bind(this)
            .Map(resources => resources.plastic)
            .Lazy()
            .Get(plasticAmount.SetResourceAmount);

        Globals.playerResources
            .Initialized
            .Bind(this)
            .Map(resources => resources.wood)
            .Lazy()
            .Get(woodAmount.SetResourceAmount);

        Globals.playerResources
            .Initialized
            .Bind(this)
            .Map(resources => resources.organic)
            .Lazy()
            .Get(organicAmount.SetResourceAmount);

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
                        canMove ? 0.6f : 1.0f,
                        0.7f * Time.deltaTime
                    );
            });
    }
}
