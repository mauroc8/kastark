
using System;
using UnityEngine;

public class StreamTest : StreamBehaviour
{
    protected override void Awake()
    {
        updateStream.Get(_ =>
        {
            //Debug.Log("Update");
        });

        enableStream
            .Map(_ => Time.time)
            .AndThen(disableStream.Always)
            .Map(time => Time.time - time)
            .Get(time =>
            {
                Debug.Log($"Time enabled (with pipe): {time}");
            });

        Stream.Combine(
            enableStream.Map(_ => Time.time),
            disableStream.Map(_ => Time.time))
            .Map((enable, disable) => disable - enable)
            .Filter(time => time > 0)
            .Get(time =>
            {
                Debug.Log($"Time enabled (with combine): {time}");
            });
    }
}
