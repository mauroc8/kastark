using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : UpdateAsStream
{
    public int id = 0;

    void Awake()
    {
        var worldScene =
            GetComponentInParent<WorldScene>();

        var worldState =
            worldScene.State;

        var isInFrontOfPlayer =
            worldState
                .Map(Optional.FromCast<InteractState, InteractStates.InFrontOfEnemy>)
                .Map(optionalInteract =>
                    optionalInteract.CaseOf(
                        interact => interact.enemy == this,
                        () => false
                    )
                )
                .Lazy();

        var inFrontOfPlayerUpdate =
            isInFrontOfPlayer
                .AndThen(value =>
                    value
                        ? update
                        : Stream.None<Void>()
                );

        var inFrontOfPlayerT =
            isInFrontOfPlayer
                .Map(t => t ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 1.3f));

        // Load Battle

        inFrontOfPlayerT
            .Get(t =>
            {
                if (t >= 1f)
                {
                    Scenes.LoadBattle(id);
                }
            });

        // Show view-player feedback

        var viewPlayer =
            Query
                .From(this, "view-player")
                .Get<TextMesh>();

        isInFrontOfPlayer
            .Get(viewPlayer.gameObject.SetActive);

        var darkScreen =
            Query
                .From(this, "dark-screen")
                .Get<Image>();

        inFrontOfPlayerT
            .InitializeWith(0.0f)
            .Map(t => t != 0.0f)
            .Lazy()
            .Get(darkScreen.gameObject.SetActive);

        inFrontOfPlayerT
            .Get(t =>
            {
                darkScreen.color =
                    new Color(
                        0.1f,
                        0.0f,
                        0.0f,
                        Mathf.Pow(t, 3.0f)
                    );
            });

        var path =
            Query
                .From(this, "path")
                .GetAll<Transform>();

        var movingOthok =
            Query
                .From(this, "moving-othok")
                .Get<UnityEngine.AI.NavMeshAgent>();

        // STATE

        var pathStatus =
            new StateStream<PathStatus>(
                new Walking { targetPoint = 1 }
            );

        // Stop walking

        update
            .Map(_ => movingOthok.remainingDistance <= 0.5f)
            .Lazy()
            .Filter(a => a)
            .Get(_ =>
            {
                switch (pathStatus.Value)
                {
                    case Walking walking:

                        pathStatus.Value =
                            new Idle
                            {
                                currentPoint = walking.targetPoint,
                                time = Time.time
                            };

                        break;
                }
            });

        pathStatus
            .AndThen(status =>
                Functions.IsTypeOf<PathStatus, Idle>(status)
                    ? update.Always((Idle)status)
                    : Stream.None<Idle>()
            )
            .Get(idle =>
            {
                var t =
                    (Time.time - idle.time) / 3.28f;

                if (t >= 1)
                {
                    pathStatus.Value =
                        new Rotating
                        {
                            currentRotation =
                                movingOthok.transform.rotation,
                            targetRotation =
                                Quaternion.FromToRotation(
                                    path[idle.currentPoint % path.Length].position,
                                    path[(idle.currentPoint + 1) % path.Length].position
                                ),
                            targetPoint =
                                idle.currentPoint + 1,
                            time =
                                Time.time
                        };
                }
            });

        var rotatingDuration =
            1.63f;

        pathStatus
            .AndThen(status =>
                Functions.IsTypeOf<PathStatus, Rotating>(status)
                    ? update.Always((Rotating)status)
                    : Stream.None<Rotating>()
            )
            .Get(rotating =>
            {
                var t =
                    (Time.time - rotating.time) / rotatingDuration;

                if (t >= 1)
                {
                    pathStatus.Value =
                        new Walking
                        {
                            targetPoint =
                                rotating.targetPoint
                        };
                }
            });

        // Walk animation

        pathStatus
            .Initialized
            .FilterMap(Optional.FromCast<PathStatus, Walking>)
            .Get(walking =>
            {
                movingOthok
                    .SetDestination(path[walking.targetPoint % path.Length].position);
            });

        var animator =
            Query
                .From(this)
                .Get<Animator>();

        pathStatus
            .Initialized
            .Map(Functions.IsTypeOf<PathStatus, Walking>)
            .Get(value =>
            {
                animator.SetFloat("horizontal_speed", value ? 1.0f : 0.0f);
            });

        // Rotation

        pathStatus
            .FilterMap(Optional.FromCast<PathStatus, Rotating>)
            .Get(rotating =>
            {
                var t =
                    (Time.time - rotating.time) / rotatingDuration;

                movingOthok.transform.rotation =
                    Quaternion.Lerp(
                        rotating.currentRotation,
                        rotating.targetRotation,
                        t
                    );
            });
    }
}

interface PathStatus
{
}

struct Walking : PathStatus
{
    public int targetPoint;
}

struct Idle : PathStatus
{
    public int currentPoint;
    public float time;
}

struct Rotating : PathStatus
{
    public Quaternion currentRotation;
    public Quaternion targetRotation;
    public int targetPoint;
    public float time;
}
