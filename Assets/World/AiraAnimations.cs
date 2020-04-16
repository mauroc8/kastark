using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AiraAnimations : MonoBehaviour
{
    void Awake()
    {
        var animationController = GetComponentInChildren<Animator>();
        var worldScene = GetComponentInParent<WorldScene>();

        worldScene.State
            .Map(state => state.playerMovement)
            .Lazy()
            .Get(playerMovement =>
            {
                animationController.SetFloat("horizontal_speed", playerMovement.radius);
            });
    }
}
