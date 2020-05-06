using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ButtonHabilitySelection : HabilitySelection
{
    TaskCompletionSourceWithAutoCancel<Hability> _taskCompletionSource;

    private StateStream<bool> isChoosingHability =
        new StateStream<bool>(true);

    void Awake()
    {
        var attackButton =
            Node.Query(this, "attack-button")
                .GetComponent<UnityEngine.UI.Button>();

        var shieldButton =
            Node.Query(this, "shield-button")
                .GetComponent<UnityEngine.UI.Button>();

        var potionButton =
            Node.Query(this, "potion-button")
                .GetComponent<UnityEngine.UI.Button>();

        var magicButton =
            Query
                .From(this, "magic-button")
                .Get<UnityEngine.UI.Button>();

        Stream.Merge(
            Globals.potions.Map(_ => new Void { }),
            isChoosingHability.Map(_ => new Void { })
        )
            .Listen(this, _ =>
            {
                var choosing =
                    isChoosingHability.Value;

                attackButton.interactable =
                    choosing && Globals.hasSword.Value;

                shieldButton.interactable =
                    choosing && Globals.hasShield.Value;

                potionButton.interactable =
                    choosing && Globals.potions.Value > 0;

                magicButton.interactable =
                    choosing && Globals.hasMagic.Value;
            });

        var amount =
            Node.Query(this, "potion-button amount")
                .GetComponent<TMPro.TextMeshProUGUI>();

        amount.text =
            $"x{Globals.potions.Value}";

        Globals
            .potions
            .Listen(this, value =>
            {
                amount.text =
                    $"x{value}";
            });
    }

    public override Task<Hability> SelectHabilityAsync(CancellationToken token)
    {
        // Hability selection start
        isChoosingHability.Push(true);

        _taskCompletionSource = new TaskCompletionSourceWithAutoCancel<Hability>(token);
        return _taskCompletionSource.Task;
    }

    public void SelectHabilityHandler(Hability hability)
    {
        if (_taskCompletionSource == null) return;

        _taskCompletionSource.SetResult(hability);
        _taskCompletionSource = null;

        // Hability Selection end
        isChoosingHability.Push(false);
    }
}