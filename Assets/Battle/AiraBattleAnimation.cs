using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiraBattleAnimation : MonoBehaviour
{
    void Awake()
    {
        var animator = GetComponentInChildren<Animator>();

        animator.SetFloat("vertical_height", 1.0f);
    }

    void OnDestroy()
    {
        var animator = GetComponentInChildren<Animator>();

        animator.SetFloat("vertical_height", 0.0f);
        animator.SetBool("is_dead", false);
    }
}
