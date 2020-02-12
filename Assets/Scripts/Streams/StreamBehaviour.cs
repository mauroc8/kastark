using NodeExtensions;
using UnityEngine;

public struct Void { }

public abstract class StreamBehaviour : StreamBehaviour<Void>
{
    public Ctx GetContext<Ctx>() where Ctx : MonoBehaviour, StreamContext, new()
    {
        return NodeHelper.GetContext<Ctx>(this);
    }

    protected Stream<Evt> GlobalEventStream<Evt>() where Evt : GlobalEvents.GlobalEvent
    {
        var stream = new StreamSource<Evt>();

        enableStream.Get(_ =>
        {
            GlobalEvents.EventController.AddListener<Evt>(stream.Push);
        });

        disableStream.Get(_ =>
        {
            GlobalEvents.EventController.RemoveListener<Evt>(stream.Push);
        });

        destroyStream.Do(stream.Destroy);

        return stream;
    }

    protected Stream<Void> AtMountTime(float time)
    {
        float mountTime = 0f;
        bool wasCalled = false;

        enableStream.Get(_ =>
        {
            mountTime = Time.time;
            wasCalled = false;
        });

        return updateStream.Filter(_ =>
        {
            if (!wasCalled && Time.time - mountTime > time)
            {
                wasCalled = true;
                return true;
            }
            return false;
        });
    }
}
