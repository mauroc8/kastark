using UnityEngine;
using System.Collections.Generic;
using ListExtensions;
using System.Linq;

public class Battle : StreamBehaviour<Void, BattleEvt>
{
    protected override void Awake()
    {
    }

    public void CreatureBeganAttack(TeamId team)
    {
        eventStream.Push(new BattleEvts.CreatureBeganAttack { team = team });
    }

    public void CreatureCeasedAttack(TeamId team)
    {
        eventStream.Push(new BattleEvts.CreatureCeasedAttack { team = team });
    }
}

// State ??

public struct BattleState
{
}

// Events ----------------

public interface BattleEvt
{
}

namespace BattleEvts
{
    public struct CreatureBeganAttack : BattleEvt
    {
        public TeamId team;
    }

    public struct CreatureCeasedAttack : BattleEvt
    {
        public TeamId team;
    }
}
