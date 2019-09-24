using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour
{
    public Creature creature = null;

    [Header("Body Parts")]
    public Transform head;
    public Transform chest;
    public Transform feet;

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

    public void Attack(CreatureController other, float damage, DamageType damageType) {
        other.ReceiveAttack(damage, damageType);
    }

    public void ReceiveAttack(float damage, DamageType damageType) {
        switch (damageType) {
            case DamageType.Physical:
            case DamageType.Magical:
            {
                damage *= damageType == DamageType.Physical ? creature.physicalResistance : creature.magicalResistance;

                if (damage > 0 && creature.shield > 0) {
                    creature.shield -= damage;
                    if (creature.shield < 0) {
                        creature.health += creature.shield;
                        creature.shield = 0;
                    }
                } else {
                    creature.health -= damage;
                }
            } break;
            case DamageType.Shield: {
                creature.shield += damage;
            } break;
            case DamageType.Heal: {
                creature.health = Mathf.Min(creature.maxHealth, creature.health + damage);
            } break;
            default: {
                Debug.Log($"No handler for damageType {damageType}");
            } break;
        }
    }
}
