using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public class GameEvent {}

    public class PartyTurnEndEvent : GameEvent {
        public TeamId teamId;
    }

    public class PlayerTurnBeginEvent : GameEvent {}
    public class EnemyTurnBeginEvent : GameEvent {}
    public class HabilitySelectEvent : GameEvent {
        public HabilityInfo habilityInfo;
    }
    public class HabilityCastEvent : GameEvent {
        public HabilityInfo habilityInfo;
    }

}
