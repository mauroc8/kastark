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

    public StateStream<bool> isMyTurn =
        new StateStream<bool>(false);

    public async Task TurnAsync(CancellationToken token)
    {
        isMyTurn.Push(true);

        if (Creature.shield.Value > 0)
            Creature.shield.Value -= 1;

        EventController.TriggerEvent(new TurnStartEvent { creature = Creature });

        var selectedHability = await HabilitySelection.SelectHabilityAsync(token);

        Creature.HabilityWasSelected(selectedHability.habilityId);

        await selectedHability.CastAsync(token);

        Creature.TurnIsAboutToEnd();

        isMyTurn.Push(false);

        Debug.Log($"Turn ended");
    }
}
