using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float health = 10;
    public float maxHealth = 10;
    public float shield = 0;

    public float physicalResistance = 0.95f;
    public float magicalResistance  = 0.95f;
    public float healMultiplier     = 0.95f;

    public bool IsAlive() {
        return health > 0;
    }

    public void Attack(Creature other, float damage, DamageType damageType) {
        other.ReceiveAttack(damage, damageType);
    }

    public void ReceiveAttack(float damage, DamageType damageType) {
        // Note: use negative damage to heal and to give shield.

        switch (damageType) {
            case DamageType.Physical:
            case DamageType.Magical:
            {
                damage *= damageType == DamageType.Physical ? physicalResistance : magicalResistance;

                if (damage > 0 && shield > 0) {
                    shield -= damage;
                    if (shield < 0) {
                        health += shield;
                        shield = 0;
                    }
                } else {
                    health -= damage;
                }
            } break;
            case DamageType.Shield: {
                shield -= damage;
            } break;
            case DamageType.Heal: {
                health = Mathf.Min(maxHealth, health - damage);
            } break;
            default: {
                Debug.Log("No damageType");
            } break;
        }
    }
}
