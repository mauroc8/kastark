using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotionController : MonoBehaviour
{
    int heal = 5;

    void OnEnable()
    {
        var creature = GetComponentInParent<Creature>();

        var consumable = Functions.NullCheck(GetComponent<Consumable>());

        var healSound =
            GetComponent<AudioSource>();

        StartCoroutine(DrinkPotionCoroutine(creature, consumable, healSound));
    }

    IEnumerator DrinkPotionCoroutine(Creature creature, Consumable consumable, AudioSource sound)
    {
        Globals.potions.Push(Globals.potions.Value - 1);

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < heal; i++)
        {
            yield return new WaitForSeconds(0.35f);

            creature.HealthPotionWasDrank(1);

            if (sound)
                Functions.PlaySwooshSound(sound);
        }

        consumable.OnCastEnd();
    }

}
