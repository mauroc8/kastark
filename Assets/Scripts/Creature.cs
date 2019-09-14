using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public float shield;

    public float attackDamage;

    public bool IsAlive() {
        return health > 0;
    }

    public void Attack(Creature other) {
        other.ReceiveAttack(this, attackDamage);
    }

    public void ReceiveAttack(Creature other, float damage) {
        if (shield > 0) {
            shield -= damage;
            if (shield < 0) {
                health += shield;
                shield = 0;
            }
        } else {
            health -= damage;
        }
    }
}
