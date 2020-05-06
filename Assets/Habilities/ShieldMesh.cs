using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldMesh : UpdateAsStream
{
    void Awake()
    {
        var creature =
            GetComponentInParent<Creature>();

        var opacity =
            creature
                .shield
                .Map(value => value > 0 ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.4f));

        //

        var alphaController =
            Query
                .From(this, "shield-mesh")
                .Get<AlphaController>();

        var maxAlpha =
            alphaController.Alpha;

        opacity
            .Get(value =>
            {
                alphaController.Alpha =
                    Functions.EaseInOut(value) * maxAlpha;
            });

        //

        var mesh =
            Query
                .From(this, "shield-mesh mesh")
                .Get();

        mesh.SetActive(false);

        opacity
            .Map(t => t != 0)
            .Lazy()
            .Get(value =>
            {
                mesh.SetActive(value);
            });
    }
}
