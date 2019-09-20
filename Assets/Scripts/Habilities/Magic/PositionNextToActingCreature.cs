using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionNextToActingCreature : MonoBehaviour
{
    void Start()
    {
        var chestTransform = GameState.actingCreature.gameObject.GetComponent<CreatureTransforms>().chest;
        var pos = Camera.main.WorldToScreenPoint(chestTransform.position);
        pos.x += (GameState.actingTeam == Team.Left ? 1 : -1) * 0.1f * Camera.main.pixelHeight;
        transform.position = pos;
    }
}
