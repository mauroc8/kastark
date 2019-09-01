using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class Party : MonoBehaviour
{
    [SerializeField] TeamId _teamId = TeamId.PlayerTeam;

    public void BeginTurn() {
        Debug.Log($"{_teamId} turn began.");
    }
}
