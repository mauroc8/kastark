using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Foot
{
    Left,
    Right
};

[RequireComponent(typeof(AudioSource))]
public class StepSound : MonoBehaviour
{
    public EventStream<Foot> steps =
        new EventStream<Foot>();

    // This methods are called from unity's animator.

    void LeftFoot()
    {
        steps.Push(Foot.Left);
    }

    void RightFoot()
    {
        steps.Push(Foot.Right);
    }

    void Awake()
    {
        var audioSources =
            GetComponentsInChildren<AudioSource>();

        var i = 0;

        var N = audioSources.Length;

        steps.Get(_ =>
        {
            var audioSource =
                audioSources[i++ % N];

            audioSource.pitch =
                0.8f + Random.Range(0, 0.6f);

            audioSource.volume =
                0.15f + Random.Range(0, 0.09f);

            audioSource.Play();
        });
    }
}
