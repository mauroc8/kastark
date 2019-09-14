using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public class GameEvent {}

    public class ChangeUIScreenEvent : GameEvent { public int move; }
    
    public class BattleStartEvent : GameEvent {}

    public class StartUnitTurnEvent  : GameEvent { }
    public class EndUnitTurnEvent    : GameEvent { }

    public class SelectedHabilityEvent : GameEvent { public HabilityId habilityId; }
    public class ConfirmSelectedHabilityEvent : GameEvent { }

    public class ChangeLanguageEvent : GameEvent {}
}
