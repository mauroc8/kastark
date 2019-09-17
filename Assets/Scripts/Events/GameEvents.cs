using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public class GameEvent {}

    public class BattleStartEvent : GameEvent {}

    public class UnitTurnStartEvent  : GameEvent { }
    public class UnitTurnEndEvent    : GameEvent { }

    public class HabilitySelectEvent : GameEvent { public HabilityId habilityId; }
    public class HabilityCastStartEvent : GameEvent { }
    public class HabilityCastEndEvent : GameEvent {
        public Creature[] targets;
        public float baseDamage; // can be a negative number to heal
        public float[] effectiveness;
        public DamageType damageType;
    }

    public class LanguageChangeEvent : GameEvent {}
}
