using UnityEngine;

public class WorldState
{
    public PolarVector2 playerMovement = PolarVector2.zero;

    public PolarVector3 cameraPosition =
        new PolarVector3(1.0f, Angle.Turn / 4, 0.0f);

    public InteractState interactState = new InteractStates.None();
}

public interface InteractState
{
    bool LockPlayerControl { get; }
}

namespace InteractStates
{
    public struct None : InteractState
    {
        public bool LockPlayerControl => false;
    }

    public struct InFrontOfNPC : InteractState
    {
        public bool LockPlayerControl => false;
    }

    public struct InFrontOfMatch3 : InteractState
    {
        public bool LockPlayerControl => false;
    }

    public struct TalkingWithNPC : InteractState
    {
        public bool LockPlayerControl => true;
    }

    public struct PlayingMatch3 : InteractState
    {
        public bool LockPlayerControl => true;
    }
}
