using System.Threading;
using System.Threading.Tasks;
using GlobalEvents;
using UnityEngine;

public class TurnStartEvent : GlobalEvent
{
    public Creature creature;
}

public class CreatureTurn : MonoBehaviour
{
    private HabilitySelection HabilitySelection => GetComponentInChildren<HabilitySelection>();

    private Creature Creature => GetComponent<Creature>();

    Creature creature;

    void Awake()
    {
        creature = GetComponentInParent<Creature>();
    }

    public async Task TurnAsync(CancellationToken token)
    {
        EventController.TriggerEvent(new TurnStartEvent { creature = Creature });

        var selectedHability = await HabilitySelection.SelectHabilityAsync(token);

        creature.HabilityWasSelected(selectedHability.habilityId);

        await selectedHability.CastAsync(token);

        creature.TurnIsAboutToEnd();
    }
}
