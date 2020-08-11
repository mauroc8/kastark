using UnityEngine;

public class AiraAbilityAnimationEvents : MonoBehaviour
{
    public EventStream<Void> magicReady =
        new EventStream<Void>();

    public EventStream<Void> magicCast =
        new EventStream<Void>();

    public EventStream<Void> swordReady =
        new EventStream<Void>();

    public EventStream<Void> swordCast =
        new EventStream<Void>();

    void MagicReady()
    {
        magicReady.Push(new Void());
    }

    void MagicCast()
    {
        magicCast.Push(new Void());
    }

    void SwordReady()
    {
        swordReady.Push(new Void());
    }

    void SwordCast()
    {
        swordCast.Push(new Void());
    }
}
