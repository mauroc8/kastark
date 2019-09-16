using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public class GameEvent {}

    public class BattleStartEvent : GameEvent {}

    public class UnitTurnStartEvent  : GameEvent { }
    public class UnitTurnEndEvent    : GameEvent { }

    public class HabilitySelectEvent : GameEvent { public HabilityId habilityId; }
    public class HabilityCastStartEvent : GameEvent { }

    public class LanguageChangeEvent : GameEvent {}
}
