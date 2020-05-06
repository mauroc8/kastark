
using System;
using UnityEngine;

public class StreamTest : StreamBehaviour
{
    protected override void Awake()
    {
        update.Get(_ =>
        {
            //Debug.Log("Update");
        });

        enable
            .Map(_ => Time.time)
            .AndThen(disable.Always)
            .Map(time => Time.time - time)
            .Get(time =>
            {
                Debug.Log($"Time enabled (with pipe): {time}");
            });

        Stream.Combine(
            enable.Map(_ => Time.time),
            disable.Map(_ => Time.time))
            .Map((enable, disable) => disable - enable)
            .Filter(time => time > 0)
            .Get(time =>
            {
                Debug.Log($"Time enabled (with combine): {time}");
            });
    }
}
