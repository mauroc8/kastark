using System;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class StreamBehaviour : MonoBehaviour
{
    protected Stream<Evt> GlobalEventStream<Evt>() where Evt : GlobalEvents.GlobalEvent
    {
        var stream = new StreamSource<Evt>();

        enable.Get(_ =>
        {
            GlobalEvents.EventController.AddListener<Evt>(stream.Push);
        });

        disable.Get(_ =>
        {
            GlobalEvents.EventController.RemoveListener<Evt>(stream.Push);
        });

        return stream;
    }

    Func<float, float> _elapsedTime =
        (float time) => Time.time - time;

    protected Stream<float> MountTimeStream(float time)
    {
        return enable
            .Map(_ => Time.time)
            .Apply(update.Always(_elapsedTime))
            .Filter(mountTime =>
                mountTime <= time &&
                    time < mountTime + Time.deltaTime);
    }
}
