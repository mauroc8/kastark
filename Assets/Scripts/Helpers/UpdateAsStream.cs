using UnityEngine;

public class UpdateAsStream : MonoBehaviour
{
    private EventStream<Void> _updateStream = new EventStream<Void>();
    protected Stream<Void> update => _updateStream;

    void Update()
    {
        _updateStream.Push(new Void());
    }
}
