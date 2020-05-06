using UnityEngine;

public class Shop : MonoBehaviour
{
    public AudioClip buyClip = null;
    public AudioClip errorClip = null;

    void Awake()
    {
        var sword =
            Node.Query(this, "sword-panel");

        var shield =
            Node.Query(this, "shield-panel");

        var potion =
            Node.Query(this, "potion-panel");

        var swordPanel =
            sword.GetComponent<ShopPanel>();

        var shieldPanel =
            shield.GetComponent<ShopPanel>();

        var potionPanel =
            potion.GetComponent<ShopPanel>();

        var swordClick =
            sword
                .GetComponentInChildren<HoverAndClickEventTrigger>()
                .click;

        var shieldClick =
            shield
                .GetComponentInChildren<HoverAndClickEventTrigger>()
                .click;

        var potionClick =
            potion
                .GetComponentInChildren<HoverAndClickEventTrigger>()
                .click;


        var buyAudio =
            gameObject.AddComponent<AudioSource>();

        buyAudio.playOnAwake = false;
        buyAudio.clip = buyClip;
        buyAudio.volume = 0.6f;

        var errorAudio =
            gameObject.AddComponent<AudioSource>();

        errorAudio.playOnAwake = false;
        errorAudio.clip = errorClip;
        errorAudio.volume = 0.6f;

        swordClick
            .Get(_ =>
            {
                if (Globals.hasSword.Value)
                    return;

                var costInPlastic = 6;

                var resources =
                    Globals.playerResources.Value;

                if (resources.plastic < costInPlastic)
                {
                    errorAudio.pitch =
                        0.9f + Random.Range(0.0f, 0.2f);
                    errorAudio.Play();

                    return;
                }

                // Play sound

                buyAudio.pitch =
                    0.9f + Random.Range(0.0f, 0.2f);
                buyAudio.Play();

                // Use resources

                resources.plastic -= costInPlastic;
                Globals.playerResources.Push(resources);

                // Get item

                Globals.hasSword.Push(true);

                // Disable button

                swordPanel.disableButton.Push(true);
            });


        shieldClick
            .Get(_ =>
            {
                if (Globals.hasShield.Value)
                    return;

                var resources =
                    Globals.playerResources.Value;

                if (resources.wood < 6)
                {
                    errorAudio.pitch =
                        0.9f + Random.Range(0.0f, 0.2f);
                    errorAudio.Play();

                    return;
                }

                // Play sound

                buyAudio.pitch =
                    0.9f + Random.Range(0.0f, 0.2f);
                buyAudio.Play();

                // Use resources

                resources.wood -= 6;
                Globals.playerResources.Push(resources);

                // Get item

                Globals.hasShield.Push(true);

                // Disable button

                shieldPanel.disableButton.Push(true);
            });


        potionClick
            .Get(_ =>
            {
                var resources =
                    Globals.playerResources.Value;

                if (resources.organic < 3)
                {
                    errorAudio.pitch =
                        0.9f + Random.Range(0.0f, 0.2f);
                    errorAudio.Play();

                    return;
                }

                // Play sound

                buyAudio.pitch =
                    0.9f + Random.Range(0.0f, 0.2f);
                buyAudio.Play();

                // Use resources

                resources.organic -= 3;
                Globals.playerResources.Push(resources);

                // Get item

                Globals.potions.Push(Globals.potions.Value + 1);

            });
    }
}