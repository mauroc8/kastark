using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMesh : MonoBehaviour, IHittable
{
    void Awake()
    {
        var shield =
            GetComponentInParent<Shield>();

        var creature =
            GetComponentInParent<Creature>();

        hit
            .Lazy()
            .Get(_ =>
            {
                creature.shield.Value =
                    Mathf.Max(0, creature.shield.Value - 1);
            });
    }

    StateStream<(Component, int)> hit =
        new StateStream<(Component, int)>((null, -1));

    public void Hit(Component source, int hitId)
    {
        hit.Value =
            (source, hitId);
    }
}
