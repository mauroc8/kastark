using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Shop : MonoBehaviour
{
    public ItemsDictionary itemsDictionary;

    enum BuyEvent
    {
        Buy,
        Error
    };

    void Awake()
    {
        var dhende =
            Query
                .From(transform.root, "ui dhende")
                .Get();

        var shop =
            Query
                .From(dhende, "shop-screen")
                .Get();

        var shopItemsTrigger =
            Query
                .From(shop, "shop-items")
                .GetAll<HoverAndClickEventTrigger>();

        var shopItemsOutline =
            Query
                .From(shop, "shop-items")
                .GetAll<Outline>();

        var shopItems =
            new Item[]
            {
                new Items.Magic(),
                new Items.Sword(),
                new Items.Shield(),
                new Items.Potion(2)
            };

        var shopItemCosts =
            new PlayerResources[]
            {
                new PlayerResources(0, 10, 0),
                new PlayerResources(6, 0, 0),
                new PlayerResources(9, 0, 2),
                new PlayerResources(8, 0, 1)
            };

        // Initialize icon and name of items

        var itemsIcons =
            shopItemsTrigger
                .Select(trigger =>
                    Query
                        .From(trigger, "icon")
                        .Get<Image>()
                )
                .ToArray();

        var itemsTitles =
            shopItemsTrigger
                .Select(trigger =>
                    Query
                        .From(trigger, "title")
                        .Get<TMPro.TextMeshProUGUI>()
                )
                .ToArray();

        for (int i = 0; i < itemsIcons.Length; i++)
        {
            itemsIcons[i].sprite =
                itemsDictionary.SpriteFromItem(shopItems[i]);

            itemsTitles[i].text =
                shopItems[i].Title;
        }

        // Disable items that are in the inventory

        Globals
            .inventory
            .Bind(this)
            .Initialized
            .Get(inventory =>
            {
                foreach (var item in inventory)
                {
                    switch (item)
                    {
                        case Items.Magic magic:
                            shopItemsTrigger[0].gameObject.SetActive(false);
                            break;
                        case Items.Sword sword:
                            shopItemsTrigger[1].gameObject.SetActive(false);
                            break;
                        case Items.Shield shield:
                            shopItemsTrigger[2].gameObject.SetActive(false);
                            break;
                        case Items.Potion potion:
                            shopItemsTrigger[3].gameObject.SetActive(false);
                            break;
                    }
                }
            });

        // Selection

        var selection =
            new StateStream<int>(-1);

        for (int i = 0; i < shopItemsTrigger.Length; i++)
        {
            int x = i;

            shopItemsTrigger[x]
                .click
                .Get(_ =>
                {
                    selection.Value =
                        x;
                });
        }

        // Show selection

        selection
            .Initialized
            .Get(value =>
            {
                for (int i = 0; i < shopItemsOutline.Length; i++)
                {
                    shopItemsOutline[i].enabled =
                        value == i;
                }
            });

        // Show information

        var selectedItemIcon =
            Query
                .From(shop, "right-col icon")
                .Get<Image>();

        var selectedItemTitle =
            Query
                .From(shop, "right-col title")
                .Get<TMPro.TextMeshProUGUI>();

        var selectedItemDescription =
            Query
                .From(shop, "right-col description")
                .Get<TMPro.TextMeshProUGUI>();

        var selectedItemResources =
            Query
                .From(shop, "right-col resources")
                .Get();

        var selectedItemOrganic =
            Query
                .From(selectedItemResources, "organic")
                .Get<TMPro.TextMeshProUGUI>();

        var selectedItemWood =
            Query
                .From(selectedItemResources, "wood")
                .Get<TMPro.TextMeshProUGUI>();

        var selectedItemPlastic =
            Query
                .From(selectedItemResources, "plastic")
                .Get<TMPro.TextMeshProUGUI>();

        var buyButton =
            Query
                .From(shop, "buy-button")
                .Get<HoverAndClickEventTrigger>();

        selection
            .Initialized
            .Get(value =>
            {
                if (value == -1)
                {
                    selectedItemTitle.text =
                        "Select an item";

                    selectedItemDescription.text =
                        "";
                }
                else
                {
                    selectedItemIcon.sprite =
                        itemsIcons[value].sprite;

                    selectedItemTitle.text =
                        shopItems[value].Title;

                    selectedItemDescription.text =
                        shopItems[value].Description;

                    selectedItemOrganic.text =
                        $"{shopItemCosts[value].organic}";

                    selectedItemWood.text =
                        $"{shopItemCosts[value].wood}";

                    selectedItemPlastic.text =
                        $"{shopItemCosts[value].plastic}";
                }

                selectedItemResources.SetActive(value != -1);
                buyButton.gameObject.SetActive(value != -1);
                selectedItemIcon.gameObject.SetActive(value != -1);
            });

        // Buy Events

        var buyEvents =
            new EventStream<BuyEvent>();

        buyButton
            .click
            .Get(_ =>
            {
                var boughtItemCost =
                    shopItemCosts[selection.Value];

                var resources =
                    Globals.playerResources.Value;

                if (resources.IsGreaterOrEqualThan(boughtItemCost))
                {
                    buyEvents.Push(BuyEvent.Buy);
                }
                else
                {
                    buyEvents.Push(BuyEvent.Error);
                }
            });

        // Buy

        buyEvents
            .Filter(evt => evt == BuyEvent.Buy)
            .Get(_ =>
            {
                var boughtItemCost =
                    shopItemCosts[selection.Value];

                var resources =
                    Globals.playerResources.Value;

                var boughtItem =
                    shopItems[selection.Value];

                var inventory =
                    Globals.inventory.Value;

                for (int i = 0; i < inventory.Length; i++)
                {
                    if (Items.Empty.IsEmpty(inventory[i]))
                    {
                        inventory[i] =
                            boughtItem;

                        break;
                    }
                }

                Globals.inventory.Value =
                    inventory;

                Globals.playerResources.Value =
                    resources.Minus(boughtItemCost);

                selection.Value =
                    -1;
            });

        // Audio

        var selectAudio =
            Query
                .From(dhende, "audio select")
                .Get<AudioSource>();

        var buyAudio =
            Query
                .From(dhende, "audio buy")
                .Get<AudioSource>();

        var buyErrorAudio =
            Query
                .From(dhende, "audio buy-error")
                .Get<AudioSource>();

        selection
            .Get(value =>
            {
                if (value == -1)
                    return;

                selectAudio.Play();
            });

        buyEvents
            .Get(evt =>
            {
                if (evt == BuyEvent.Buy)
                    buyAudio.Play();
                else
                    buyErrorAudio.Play();
            });

        // Buy error

        var buyErrorInfo =
            Query
                .From(shop, "buy-error-info")
                .Get();

        buyEvents
            .Filter(evt => evt == BuyEvent.Error)
            .Get(_ =>
            {
                buyErrorInfo.SetActive(true);
            });

        selection
            .Initialized
            .Get(_ =>
            {
                buyErrorInfo.SetActive(false);
            });
    }
}