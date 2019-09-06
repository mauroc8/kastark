using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public class GameEvent {}

    public class ChangeUIScreenEvent : GameEvent { public int move; }
    public class StartUnitTurnEvent  : GameEvent { public GameObject unit; }
    public class EndUnitTurnEvent    : GameEvent { }
}
