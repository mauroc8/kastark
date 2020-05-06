using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Hability))]
public class Shield : MonoBehaviour
{
    void OnEnable()
    {
        var creature =
            GetComponentInParent<Creature>();

        creature.shield.Value = 2;

        var hability =
            GetComponent<Hability>();

        StartCoroutine(CastEnd(hability));
    }

    IEnumerator CastEnd(Hability hability)
    {
        yield return null;
        hability.OnCastEnd();
    }
}
