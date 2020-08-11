using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ButtonHabilitySelection : MonoBehaviour
{
    public ItemsDictionary itemsDictionary = null;

    [Header("Abilities")]
    public Ability swordAbility = null;
    public Ability magicAbility = null;
    public Ability shieldAbility = null;
    public Ability potionAbility = null;

    public Ability smallSword = null;
    public Ability smallMagic = null;

    Ability AbilityFromItem(Item item)
    {
        switch (item)
        {
            case Items.Sword _:
                return swordAbility;

            case Items.Magic _:
                return magicAbility;

            case Items.Shield _:
                return shieldAbility;

            case Items.Potion _:
                return potionAbility;

            case Items.SmallSword _:
                return smallSword;

            case Items.SmallMagic _:
                return smallMagic;

            default:
                return null;
        }
    }

    void Awake()
    {
        var abilityBar =
            Query
                .From(this, "ability-bar")
                .Get();

        var slots =
            GetComponentsInChildren<AbilityBarSlot>();

        var slotIcons =
            slots
                .Select(slot =>
                    Query
                        .From(slot, "icon")
                        .Get<Image>()
                )
                .ToArray();

        var slotTitles =
            slots
                .Select(slot =>
                    Query
                        .From(slot, "title")
                        .Get<TMPro.TextMeshProUGUI>()
                )
                .ToArray();

        var slotDescriptions =
            slots
                .Select(slot =>
                    Query
                        .From(slot, "description")
                        .Get<TMPro.TextMeshProUGUI>()
                )
                .ToArray();

        var battle =
            GetComponentInParent<Battle>();

        var creature =
            GetComponentInParent<Creature>();

        var isChoosingHability =
            battle
                .turn
                .Map(turn =>
                    turn
                        .CaseOf(
                            turnValue =>
                                battle.ActingCreature(turnValue) == creature
                                    && turnValue.action == TurnAction.SelectAbility,
                            () => false
                        )
                )
                .Lazy();

        isChoosingHability
            .InitializeWith(false)
            .Get(choosing =>
            {
                abilityBar.SetActive(choosing);
            });

        var startAbilityBarIdx = 6;

        Globals
            .inventory
            .Bind(this)
            .Initialized
            .Get(items =>
            {
                for (var i = 0; i + startAbilityBarIdx < items.Length; i++)
                {
                    var item =
                        FromInventory(i);

                    slots[i]
                        .gameObject
                        .SetActive(
                            !Items.Empty.IsEmpty(item)
                        );

                    slotIcons[i].sprite =
                        itemsDictionary.SpriteFromItem(item);

                    slotTitles[i].text =
                        item.Title;

                    slotDescriptions[i].text =
                        item.Description;
                }
            });

        // Hover info

        var slotEventTriggers =
            slots
                .Select(slot =>
                    Query
                        .From(slot)
                        .Get<HoverAndClickEventTrigger>()
                )
                .ToArray();

        var slotHovers =
            slotEventTriggers
                .Select(eventTrigger =>
                    eventTrigger.isHovering
                )
                .ToArray();

        var slotHoverInfos =
            slots
                .Select(slot =>
                    Query
                        .From(slot, "hover-info")
                        .Get()
                )
                .ToArray();

        Stream
            .FromArray(slotHovers)
            .Get((isHovering, i) =>
            {
                slotHoverInfos[i]
                    .SetActive(isHovering);
            });

        // Click

        var slotClicks =
            slotEventTriggers
                .Select(eventTrigger =>
                    eventTrigger.click
                )
                .ToArray();

        Stream
            .FromArray(slotClicks)
            .Get((_, i) =>
            {
                var item =
                    FromInventory(i);

                switch (item)
                {
                    case Items.Shield _:
                        if (creature.shield.Value > 0)
                        {
                            // We do not cast a shield twice!
                            return;
                        }
                        break;
                }

                var ability =
                    AbilityFromItem(item);

                if (ability != null)
                {
                    battle.CreatureSelectsAbility(ability);
                }
            });

        // Disable shield

        creature
            .shield
            .Map(value => value != 0)
            .Lazy()
            .Get(hasShield =>
            {
                var items =
                    Globals.inventory.Value;

                for (var i = 0; i + startAbilityBarIdx < items.Length; i++)
                {
                    var item =
                        FromInventory(i);

                    switch (item)
                    {
                        case Items.Shield _:

                            slotIcons[i].color =
                                hasShield
                                    ? Color.gray
                                    : Color.white;

                            return;
                    }
                }
            });
    }

    public void SelectAbilityHandler(Ability ability)
    {
        var battle =
            GetComponentInParent<Battle>();

        battle.CreatureSelectsAbility(ability);
    }

    Item FromInventory(int index)
    {
        var startAbilityBarIdx = 6;

        var item =
            Globals.inventory.Value[startAbilityBarIdx + index];

        if (Items.Empty.IsEmpty(item))
        {
            if (index == 0)
                return new Items.SmallSword();
            if (index == 1)
                return new Items.SmallMagic();
        }

        return item;
    }
}