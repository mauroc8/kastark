using UnityEngine;

public class UpdateAsStream : MonoBehaviour
{
    private StreamSource<Void> _updateStream = new StreamSource<Void>();
    protected Stream<Void> update => _updateStream;

    void Update()
    {
        _updateStream.Push(new Void());
    }
}
