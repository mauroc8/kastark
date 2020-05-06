using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clover : MonoBehaviour
{
    public void PlayerWins()
    {
        var mesh =
            Query.From(this, "trebol").Get();

        mesh.SetActive(true);
    }
}
