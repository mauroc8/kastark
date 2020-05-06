using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

public class WorldLoad : MonoBehaviour
{
    void Awake()
    {
        var othok =
            Node.Query(this, "othok");

        var clover =
            Node.Query(this, "clover");

        var player =
            Node.Query(this, "player");

        var spawnAfterBattle =
            Node.Query(this, "spawn-after-battle");
    }
}
