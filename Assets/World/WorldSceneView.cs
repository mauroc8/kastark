using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

[RequireComponent(typeof(WorldScene))]
public class WorldSceneView : StreamBehaviour
{
    protected override void Awake()
    {
        var worldScene =
            GetComponent<WorldScene>();

        var worldState =
            worldScene.State;

        var playerCanMove =
            worldState
                .Map(state => !state.interactState.LockPlayerControl);

        var playerCanMoveUpdate =
            playerCanMove
                .AndThen(canMove => canMove ? update : Stream.None<Void>());

        // Lock/unlock cursor.

        playerCanMove.Get(canMove =>
        {
            Cursor.lockState = canMove ? CursorLockMode.Locked : CursorLockMode.None;
        });

        // Camera follows creature

        var camera = GetComponentInChildren<Camera>();

        var cameraController =
            Node
                .Query(this, "camera-collider")
                .GetComponent<CharacterController>();

        var cameraFocus = Node.Query(this, "camera-focus");

        worldState
            .Map(state => state.cameraPosition)
            .Lazy()
            .AndThen(playerCanMoveUpdate.Always)
            .Get(cameraPosition =>
            {
                var elevation =
                    Functions.EaseInOutNth(cameraPosition.elevation, 1.2f);

                var radius =
                    Functions.EaseInOutNth(cameraPosition.radius, 0.6f);

                // Change Field Of View.

                camera.fieldOfView =
                    Mathf.Lerp(
                        camera.fieldOfView,
                        Mathf.Lerp(
                            16.0f,
                            19.0f,
                            radius
                        ),
                        6.2f * Time.deltaTime
                    )
                ;

                cameraPosition =
                    cameraPosition
                        .WithElevation(
                            Mathf.Lerp(
                                0.02f * Angle.Turn,
                                0.2f * Angle.Turn,
                                elevation
                            ))
                        .WithRadius(
                            Mathf.Lerp(
                                200.0f,
                                320.0f,
                                radius
                            )
                        )
                        .WithRotation(
                            Angle.Turn / 2
                                + cameraPosition.rotation
                        );

                // Move Controller first.
                //
                //cameraController.Move(
                //    (cameraPosition.ToVector3()
                //        - cameraController.transform.localPosition)
                //);

                // The camera follows the controller (eased in spherical space).

                camera.transform.localPosition =
                    PolarVector3.Lerp(
                        PolarVector3.FromVector3(camera.transform.localPosition),
                //        PolarVector3.FromVector3(cameraController.transform.localPosition),
                        cameraPosition,
                        5.2f * Time.deltaTime
                    ).ToVector3();

                camera.transform.LookAt(cameraFocus.transform, Vector3.up);
            });


        // Aira moves.

        var playerMoveController =
            Node.Query(this, "player").GetComponent<CharacterController>();

        worldState
            .Map(state => state.playerMovement)
            .Lazy()
            .AndThen(playerCanMoveUpdate.Always)
            .Get(movement =>
            {
                playerMoveController.SimpleMove(
                    new PolarVector3(-movement.radius, movement.rotation, 0.0f).ToVector3()
                );
            });


        // Aira rotates towards her movement

        var aira = Node.Query(this, "aira");

        worldState
            .Map(state => state.playerMovement.rotation)
            .Lazy()
            .AndThen(update.Always)
            .Accumulate(0.0f, (lastRotation, rotation) =>
            {
                return Angle.Lerp(lastRotation, rotation, 12.5f * Time.deltaTime);
            })
            .Get(rotation =>
            {
                aira.transform.rotation =
                    Quaternion.LookRotation(
                        new PolarVector3(-1.0f, rotation, 0.0f).ToVector3(),
                        Vector3.up
                    );
            });


        // Aira flies up and down
        /*
        update
            .Get(time =>
            {
                var cycleDuration = 2.7f;

                aira.transform.localPosition =
                    aira.transform.localPosition.WithY(
                        Mathf.Lerp(
                            1.8f,
                            3.5f,
                            0.5f + 0.5f * Mathf.Sin(Time.time / cycleDuration * Angle.Turn)
                        )
                    );
            });
        */


        // Show "(E) Talk" text when Aira is in front of NPC.

        var npc = Node.Query(this, "npc");

        var npcInteractText = Node.Query(npc.transform, "interact-text");

        worldState
            .Map(state => state.interactState)
            .Map(Functions.IsTypeOf<InteractState, InteractStates.InFrontOfNPC>)
            .Lazy()
            .Get(inFrontOfNPC =>
            {
                npcInteractText.SetActive(inFrontOfNPC);
            });

        // --- Talk with NPC ---

        var isTalkingToNpc =
            worldState
                .Map(state => state.interactState)
                .Map(Functions.IsTypeOf<InteractState, InteractStates.TalkingWithNPC>)
                .Lazy();

        var talkingToNpcUpdate =
            isTalkingToNpc
                .AndThen(talking =>
                    talking ? update : Stream.None<Void>()
                );

        // Move camera

        var npcCameraTransform =
            Node.Query(npc.transform, "camera-transform");

        talkingToNpcUpdate.Do(() =>
        {
            camera.transform.position =
                Vector3.Lerp(
                    camera.transform.position,
                    npcCameraTransform.transform.position,
                    5.2f * Time.deltaTime
                );

            camera.transform.rotation =
                Quaternion.Lerp(
                    camera.transform.rotation,
                    npcCameraTransform.transform.rotation,
                    5.2f * Time.deltaTime
                );
        });

        // Show/hide dialog panel

        var dialogPanel =
            Node.Query(npc.transform, "dialog-panel");

        var dialogPanelAlpha =
            dialogPanel.GetComponent<AlphaController>();

        isTalkingToNpc.Get(talking =>
        {
            dialogPanel.SetActive(talking);

            if (talking && dialogPanelAlpha != null)
                dialogPanelAlpha.FadeIn(0.6f, 2f);
        });



        // Show "(E) Recycle" text

        var match3 = Node.Query(this, "match3");

        var match3InteractText = Node.Query(match3.transform, "interact-text");

        worldState
            .Map(state => state.interactState)
            .Map(Functions.IsTypeOf<InteractState, InteractStates.InFrontOfMatch3>)
            .Lazy()
            .Get(inFrontOfMatch3 =>
            {
                match3InteractText.SetActive(inFrontOfMatch3);
            });

        // Begin Match3

        var match3CameraTransform =
            Node.Query(match3.transform, "camera-transform");

        var isPlayingMatch3 =
            worldState
                .Map(state => state.interactState)
                .Map(Functions.IsTypeOf<InteractState, InteractStates.PlayingMatch3>)
                .Lazy();


        var playingMatch3Update =
            isPlayingMatch3
                .AndThen(playingMatch3 =>
                    playingMatch3 ? update : Stream.None<Void>());

        playingMatch3Update
            .Do(() =>
            {
                camera.transform.position =
                    Vector3.Lerp(
                        camera.transform.position,
                        match3CameraTransform.transform.position,
                        5.2f * Time.deltaTime
                    );

                camera.transform.rotation =
                    Quaternion.Lerp(
                        camera.transform.rotation,
                        match3CameraTransform.transform.rotation,
                        5.2f * Time.deltaTime
                    );
            });


        isPlayingMatch3.Get(playing =>
        {
            aira.SetActive(!playing);
        });

        var exitInteractionText = Node.Query(this, "exit-text");

        playerCanMove.Get(canMove =>
        {
            exitInteractionText.SetActive(!canMove);
        });
    }
}
