using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class CreatureController : MonoBehaviour
{
    public Creature creature = null;

    [Header("Body Parts")]
    public Transform head;
    public Transform chest;
    public Transform feet;

    [Header("UI")]
    [SerializeField] CreatureColorEffects _colorController = null;

    void Awake()
    {
        creature.Init(this);
    }

    public Transform GetBodyPart(BodyPart bodyPart)
    {
        return bodyPart == BodyPart.Head  ? head :
               bodyPart == BodyPart.Chest ? chest :
               bodyPart == BodyPart.Feet  ? feet :
               null;
    }

    public bool IsAlive() {
        return creature.health > 0;
    }

    public void ReceiveDamage(float damage)
    {
        if (creature.shield > 0)
        {
            damage = Mathf.Max(0, damage - creature.shield);
            creature.shield = 0;
        }
        creature.health -= damage;
    }

    public void ReceiveAttack(float damage)
    {
        ReceiveDamage(damage * creature.physicalResistance);
        _colorController.ReceiveDamage(damage);
    }

    public void ReceiveMagic(float damage)
    {
        ReceiveDamage(damage * creature.magicalResistance);
        _colorController.ReceiveDamage(damage);
    }

    public void ReceiveShield(float shield)
    {
        creature.shield += shield;
        _colorController.ReceiveShield(shield);
    }

    public void ReceiveHeal(float heal)
    {
        creature.health = Mathf.Min(creature.health + heal, creature.maxHealth);
        _colorController.ReceiveHeal(heal);
    }

    void OnEnable()
    {
        EventController.AddListener<TurnStartEvent>(OnTurnStart);
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnDisable()
    {
        EventController.RemoveListener<TurnStartEvent>(OnTurnStart);
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnTurnStart(TurnStartEvent evt)
    {
        if (Global.actingCreature == this)
        {
            creature.TurnStart();
        }
    }

    void OnHabilityCast(HabilityCastEvent evt)
    {
        if (Global.actingCreature == this)
        {
            StartCoroutine(CastHabilityCoroutine());
        }
    }

    WaitForSeconds _castHabilityWait = new WaitForSeconds(0.3f);

    IEnumerator CastHabilityCoroutine()
    {
        yield return _castHabilityWait;
        EventController.TriggerEvent(new TurnEndEvent());
    }
}
