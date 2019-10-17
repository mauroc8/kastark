using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class SetVideoPlayerMainCamera : MonoBehaviour
{
    void Awake()
    {
        var videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.targetCamera = Camera.main;
    }
}
