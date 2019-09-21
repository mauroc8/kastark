using System.Collections.Generic;
using UnityEngine;

namespace Events{
    public class GameEvent {}

    public class BattleStartEvent : GameEvent {}

    public class TurnStartEvent  : GameEvent { }
    public class TurnEndEvent    : GameEvent { }

    public class HabilitySelectEvent : GameEvent { public Hability hability; }
    public class HabilityCancelEvent : GameEvent {}

    public class HabilityCastEvent : GameEvent
    {
        private CreatureController[] _targets;
        private float _damage;
        private float[] _effectiveness;
        private DamageType _damageType;

        public CreatureController[] Targets => _targets;
        public float BaseDamage => _damage; // can be a negative number to heal
        public float[] Effectiveness => _effectiveness;
        public DamageType DamageType => _damageType;
        
        public HabilityCastEvent(CreatureController target, float effectiveness)
        {
            Debug.Log($"Creating an HabilityCastEvent. Selected hability is {GameState.selectedHability?.name}.");

            _targets = new CreatureController[]{target};
            _effectiveness = new float[]{effectiveness};
            _damage = GameState.selectedHability.Damage;
            _damageType = GameState.selectedHability.DamageType;
        }

        public HabilityCastEvent(CreatureController[] targets, float[] effectiveness)
        {
            _targets = targets;
            _effectiveness = effectiveness;
            _damage = GameState.selectedHability.Damage;
            _damageType = GameState.selectedHability.DamageType;
        }
    }

    public class LanguageChangeEvent : GameEvent {}
}
