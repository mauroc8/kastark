using UnityEngine;
using System;
using System.Collections;
using UnityEngine.AI;

public class Player : UpdateAsStream
{
    PolarVector3 movement =
        new PolarVector3(0.0f, 135 * Angle.Degrees, 0.0f);

    float targetSpeed =
        0.0f;

    public Stream<bool> canMove { get; protected set; }

    void OnDestroy()
    {
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible =
            true;
    }

    void Awake()
    {
        canMove =
            GetComponentInParent<WorldScene>().State
            .Map(state => !state.LockPlayerControl)
            .Lazy();

        var canMoveUpdate =
            canMove
                .AndThen(value =>
                {
                    if (value)
                    {
                        var time = Time.time;

                        return update
                            .Filter(_ => Time.time - time >= 0.6f);
                    }

                    return Stream.None<Void>();
                });

        var maxSpeed = 40.0f / 1.4f * 1.6f;

        var acceleration = 9.3f;

        var lastSpeed =
            0.0f;

        // Lock cursor

        canMove
            .Get(value =>
            {
                Cursor.lockState =
                    value
                        ? CursorLockMode.Locked
                        : CursorLockMode.None;
                Cursor.visible =
                    !value;
            });

        // Lock camera

        var camera = Camera.main;

        var cameraInitialPosition =
            camera.transform.localPosition;

        var cameraInitialRotation =
            camera.transform.localRotation;

        /*
        canMove
            .AndThen(value =>
                value
                    ? update
                    : Stream.None<Void>()
            )
            .Get(_ =>
            {
                camera.transform.localPosition =
                    Vector3.Lerp(
                        camera.transform.localPosition,
                        cameraInitialPosition,
                        4.2f * Time.deltaTime
                    );

                camera.transform.localRotation =
                    Quaternion.Lerp(
                        camera.transform.localRotation,
                        cameraInitialRotation,
                        4.2f * Time.deltaTime
                    );
            });
        */

        // Calculate and set movement

        Vector2 input = Vector2.zero;

        var cameraVariables =
            Query
                .From(this, "camera-focus")
                .Get<CameraController>()
                .variables;

        canMoveUpdate
            .Get(_ =>
            {
                input =
                    new Vector2(
                        Input.GetKey(KeyCode.D) ? 1.0f :
                        Input.GetKey(KeyCode.A) ? -1.0f :
                        0.0f,
                        Input.GetKey(KeyCode.W) ? 1.0f :
                        Input.GetKey(KeyCode.S) ? -1.0f :
                        0.0f
                    );

                targetSpeed =
                    maxSpeed *
                        Mathf.Min(
                            1.0f,
                            Mathf.Sqrt(input.x * input.x + input.y * input.y)
                        );

                var speed =
                    Mathf.Max(
                        targetSpeed,
                        Mathf.Lerp(lastSpeed, targetSpeed, acceleration * Time.deltaTime)
                    );

                lastSpeed = speed;

                var rotation =
                    targetSpeed > 0.3f
                        ? Mathf.Atan2(input.y, input.x)
                            - cameraVariables.Value.rotation * Angle.Degrees
                        : movement.rotation;

                movement =
                    new PolarVector3(
                        speed,
                        rotation,
                        0.0f
                    );
            });

        // Player moves.

        var navMeshAgent =
            GetComponentInChildren<NavMeshAgent>();

        canMoveUpdate
            .Get(_ =>
            {
                navMeshAgent.Move(
                    movement.ToVector3() * Time.deltaTime
                );
            });

        // Aira changes its animation

        var animationController =
            GetComponentInChildren<Animator>();

        canMoveUpdate
            .Get(_ =>
            {
                animationController.SetFloat("horizontal_speed", input.magnitude);
            });

        canMove
            .Filter(a => !a)
            .Get(_ =>
            {
                animationController.SetFloat("horizontal_speed", 0.0f);
            });

        // Aira Rotates

        var airaTransform =
            Query.From(this, "aira").Get<Transform>();

        var lastRotation =
            movement.rotation;

        airaTransform.rotation =
            Quaternion.Euler(0, -movement.rotation / Angle.Degrees, 0);

        canMoveUpdate
            .Get(_ =>
            {
                var airaRotation =
                    Angle.Lerp(lastRotation, movement.rotation, 9.3f * Time.deltaTime);

                lastRotation =
                    airaRotation;

                airaTransform.rotation =
                    Quaternion.Euler(0, -airaRotation / Angle.Degrees, 0);
            });
    }
}

