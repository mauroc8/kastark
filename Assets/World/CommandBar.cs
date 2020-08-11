using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandBar : MonoBehaviour
{
    void Awake()
    {
        var worldScene =
            GetComponentInParent<WorldScene>();

        var worldState =
            worldScene.State;

        var commandBar =
            Query
                .From(this, "command-bar")
                .Get();

        var iInventory =
            Query
                .From(commandBar, "i-inventory")
                .Get();

        var qQuest =
            Query
                .From(commandBar, "q-quest")
                .Get();

        worldState
            .Map(state => state.LockPlayerControl)
            .Get(lockPlayerControl =>
            {
                iInventory.SetActive(!lockPlayerControl);
                qQuest.SetActive(Globals.quest != QuestStatus.None && !lockPlayerControl);
            });
    }
}
