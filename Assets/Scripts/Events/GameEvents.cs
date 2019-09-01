using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public class GameEvent {}

    public class PartyTurnEndEvent : GameEvent {
        TeamId teamId;
    }
}
