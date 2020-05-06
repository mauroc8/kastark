using UnityEngine;

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

        public Board board;
    }

    public struct TalkingWithNPC : InteractState
    {
        public bool LockPlayerControl => true;
    }

    public struct PlayingMatch3 : InteractState
    {
        public bool LockPlayerControl => true;

        public Board board;
    }

    public struct InFrontOfEnemy : InteractState
    {
        public bool LockPlayerControl => false;

        public EnemyId enemyId;

        public InFrontOfEnemy(EnemyId id)
        {
            this.enemyId = id;
        }
    }

    public struct TalkingWithEnemy : InteractState
    {
        public bool LockPlayerControl => true;

        public EnemyId enemyId;

        public TalkingWithEnemy(EnemyId id)
        {
            this.enemyId = id;
        }
    }

    public struct ViewingJournal : InteractState
    {
        public bool LockPlayerControl => true;
    }

    public struct Popup : InteractState
    {
        public bool LockPlayerControl => true;
    }

    public struct EscMenu : InteractState
    {
        public bool LockPlayerControl => true;
    }
}
