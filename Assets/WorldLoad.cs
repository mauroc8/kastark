using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class WorldLoad : MonoBehaviour
{
    void Awake()
    {
        var spawnAfterBattle =
            Node.Query(this, "spawn-after-battle");

        var player =
            Node.Query(this, "player");

        if (WorldBattleCommunication.hasEnteredBattle)
        {
            // We're returning from a battle!
            WorldBattleCommunication.hasEnteredBattle = false;

            player.transform.position =
                player.transform.position
                    .WithX(spawnAfterBattle.transform.position.x)
                    .WithZ(spawnAfterBattle.transform.position.z)
                ;
        }

        var othok =
            Node.Query(this, "othok");

        var clover =
            Node.Query(this, "clover");

        if (WorldBattleCommunication.hasWonBattle)
        {
            othok.SetActive(false);
            clover.SetActive(true);
        }
    }
}
