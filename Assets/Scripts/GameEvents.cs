using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class GameEvent { }

    public class BattleStartEvent : GameEvent { }

    public class TurnStartEvent : GameEvent
    {
        public Creature actingCreature;
    }
    public class TurnEndEvent : GameEvent { }

    //public class HabilitySelectEvent : GameEvent { public CastHabilityController hability; }
    //public class ConsumableSelectEvent : GameEvent { public CastConsumableController consumable; }
    public class HabilityCancelEvent : GameEvent { }

    public class HabilityCastEvent : GameEvent { }

    public class LanguageChangeEvent : GameEvent { }
}
