using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Globals;
using InventoryMouse;

namespace InventoryMouse
{
    public interface Base { }

    public class None : Base { }

    public class Hover : Base
    {
        public int index;
    }

    public class Drag : Base
    {
        public Vector3 offset;
        public int index;

        public float time;
    }

    public class Drop : Base
    {
        public Vector3 offset;
        public int dragIndex;
        public int dropIndex;
        public float time;
    }

    public class Cancel : Base
    {
        public Vector3 offset;
        public int index;
        public float time;
    }
}

public class Inventory : UpdateAsStream
{
    public ItemsDictionary itemsDictionary = null;

    void Awake()
    {
        var worldScene =
            GetComponent<WorldScene>();

        var worldState =
            worldScene.State;

        var isInteracting =
            worldState
                .Map(Functions.IsTypeOf<InteractState, InteractStates.ViewingInventory>)
                .Lazy();

        var interactUpdate =
            isInteracting
                .AndThen(value =>
                    value
                        ? update
                        : Stream.None<Void>()
                );

        var lerpedInteracting =
            isInteracting
                .Map(value => value ? 1.0f : 0.0f)
                .AndThen(Functions.LerpStreamOverTime(update, 0.3f));

        // Show

        var inventory =
            Query
                .From(this, "ui inventory")
                .Get();

        lerpedInteracting
            .Map(t => t != 0)
            .Lazy()
            .InitializeWith(false)
            .Get(inventory.SetActive);

        var inventoryGroup =
            Query
                .From(inventory)
                .Get<CanvasGroup>();

        lerpedInteracting
            .Get(t =>
            {
                inventoryGroup.alpha =
                    Mathf.Pow(t, 2.0f);
            });

        var inventoryScreen =
            Query
                .From(this, "ui inventory-screen")
                .Get();

        isInteracting
            .Initialized
            .Get(inventoryScreen.SetActive);


        // --- MOUSE ---

        var mouse =
            new StateStream<InventoryMouse.Base>(
                new InventoryMouse.None()
            );

        // Escape/return

        var escReturnClickUnfiltered =
            Query
                .From(this, "esc-return")
                .Get<HoverAndClickEventTrigger>()
                .click
                .Always(new Void());

        var escReturnClick =
            isInteracting
                .AndThen(value =>
                    value
                        ? escReturnClickUnfiltered
                        : Stream.None<Void>()
                );

        Stream.Merge(
            interactUpdate
                .Filter(_ => Input.GetKeyDown(KeyCode.Escape))
            ,
            escReturnClick
        )
            .Get(_ =>
            {
                worldScene.ExitInteractState();
            });


        // Slots

        var slots =
            Query
                .From(inventoryScreen)
                .GetAll<InventorySlot>()
                .Concat(
                    Query
                        .From(inventory)
                        .GetAll<InventorySlot>()
                )
                .ToArray();

        var inventorySlots =
            6;

        var totalSlots =
            10;

        var swordSlotIndex =
            6;

        var magicSlotIndex =
            7;

        if (slots.Length != totalSlots)
        {
            Debug.LogError($"Error fetching inventory slots ({slots.Length}).");
        }

        for (int i = 0; i < totalSlots; i++)
        {
            slots[i].index = i;
        }

        // Items

        var emptyBackgrounds =
            slots
                .Select((slot, _) =>
                    Query.From(slot, "background-empty").Get()
                )
                .ToArray();

        var fullBackgrounds =
            slots
                .Select((slot, _) =>
                    Query.From(slot, "background-full").Get()
                )
                .ToArray();

        var icons =
            slots
                .Select((slot, _) =>
                    Query.From(slot, "icon").Get()
                )
                .ToArray();


        var iconImages =
            icons
                .Select((icon, _) => icon.GetComponent<Image>())
                .ToArray();

        Globals
            .inventory
            .Initialized
            .Bind(this)
            .Get(inventoryValue =>
            {
                for (var i = 0; i < inventoryValue.Length; i++)
                {
                    var item =
                        inventoryValue[i];

                    var isEmpty =
                        Items.Empty.IsEmpty(item);

                    emptyBackgrounds[i].SetActive(isEmpty);
                    fullBackgrounds[i].SetActive(!isEmpty);

                    var sprite =
                        itemsDictionary.SpriteFromItem(item);

                    iconImages[i].sprite =
                        sprite;

                    icons[i].SetActive(sprite != null);
                }
            });

        Action<int, bool> toggleSlot = (i, active) =>
        {
            emptyBackgrounds[i].SetActive(!active);
            fullBackgrounds[i].SetActive(active);
            icons[i].SetActive(active);
        };

        mouse
            .WithLastValue(mouse.Value)
            .Get((lastMouse, mouseValue) =>
            {
                switch (mouseValue)
                {
                    case Drag drag:
                        toggleSlot(drag.index, false);
                        break;

                    case Drop drop:
                        toggleSlot(drop.dragIndex, false);
                        toggleSlot(drop.dropIndex, false);
                        break;

                    case Cancel cancel:
                        toggleSlot(cancel.index, false);
                        break;
                }

                switch (lastMouse)
                {
                    case Cancel cancel:
                        toggleSlot(cancel.index, true);
                        break;
                }
            });

        // Click to drag

        interactUpdate
            .Filter(_ => Input.GetMouseButtonDown(0))
            .Get(_ =>
            {
                switch (mouse.Value)
                {
                    case Hover m:
                        if (!Items.Empty.IsEmpty(Globals.inventory.Value[m.index]))
                            mouse.Value =
                                new Drag
                                {
                                    index =
                                        m.index,
                                    offset =
                                        Input.mousePosition - icons[m.index].transform.position,
                                    time =
                                        Time.time
                                };

                        break;
                }
            });

        // Hover

        var graphicRaycaster =
            inventory.GetComponentInParent<GraphicRaycaster>();

        var eventSystem =
            Query.From(transform.root).Get<EventSystem>();

        interactUpdate
            .Filter(_ => !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonUp(0))
            .Get(_ =>
            {
                if (Functions.IsTypeOf<Base, None>(mouse.Value) ||
                    Functions.IsTypeOf<Base, Hover>(mouse.Value))
                {
                    switch (HoveredInventorySlot(graphicRaycaster, eventSystem, Input.mousePosition))
                    {
                        case Some<InventorySlot> s:
                            mouse.Value =
                                new Hover { index = s.Value.index };

                            break;

                        default:
                            mouse.Value =
                                new None { };

                            break;
                    }
                }
            });

        // Drop

        interactUpdate
            .Filter(_ => Input.GetMouseButtonUp(0))
            .Get(_ =>
            {
                if (Functions.IsTypeOf<Base, Drag>(mouse.Value))
                {
                    var drag =
                        (Drag)mouse.Value;

                    var cancel =
                        new Cancel
                        {
                            time = Time.time,
                            index = drag.index,
                            offset = Input.mousePosition - drag.offset
                        };

                    switch (HoveredInventorySlot(graphicRaycaster, eventSystem, Input.mousePosition))
                    {
                        case Some<InventorySlot> s:
                            var dropIdx =
                                s.Value.index;

                            var dragItem =
                                Globals.inventory.Value[drag.index];

                            var drop =
                                new Drop
                                {
                                    dragIndex = drag.index,
                                    dropIndex = dropIdx,
                                    offset = Input.mousePosition - drag.offset,
                                    time = Time.time
                                };

                            switch (dragItem)
                            {
                                case Items.Sword _:
                                    if (dropIdx < inventorySlots || dropIdx == swordSlotIndex)
                                        mouse.Value =
                                            drop;

                                    else
                                        mouse.Value =
                                            cancel;

                                    break;

                                case Items.Magic _:
                                    if (dropIdx < inventorySlots || dropIdx == magicSlotIndex)
                                        mouse.Value =
                                            drop;

                                    else
                                        mouse.Value =
                                            cancel;

                                    break;

                                default:

                                    if (dropIdx < inventorySlots
                                        || (
                                            dragItem.CanEquip
                                                && dropIdx != inventorySlots + 0
                                                && dropIdx != inventorySlots + 1
                                           )
                                       )
                                    {
                                        mouse.Value =
                                            drop;
                                    }
                                    else
                                    {
                                        mouse.Value =
                                            cancel;
                                    }

                                    break;
                            }

                            break;

                        default:
                            mouse.Value =
                                cancel;

                            break;
                    }
                }
            });

        // Drop

        var dragView =
            Query
                .From(inventory, "drag-view")
                .Get();

        var dragViewImage =
            Query
                .From(inventory, "drag-view")
                .Get<Image>();

        var dropView =
            Query
                .From(inventory, "drop-view")
                .Get();

        var dropViewImage =
            Query
                .From(inventory, "drop-view")
                .Get<Image>();

        mouse
            .Map(Functions.IsTypeOf<Base, Drop>)
            .Lazy()
            .Get(dropView.SetActive);

        mouse
            .FilterMap(Optional.FromCast<Base, Drop>)
            .Get(drop =>
            {
                var sprite =
                    iconImages[drop.dropIndex].sprite;

                dropViewImage.sprite =
                    sprite;

                dropView.SetActive(sprite != null);
            });

        mouse
            .AndThen(mouseValue =>
                Functions.IsTypeOf<Base, Drop>(mouseValue)
                    ? interactUpdate.Always((Drop)mouseValue)
                    : Stream.None<Drop>()
            )
            .Get(drop =>
            {
                var dragIdx =
                    drop.dragIndex;

                var dropIdx =
                    drop.dropIndex;

                var t =
                    Functions.EaseInOut(
                        (Time.time - drop.time) / 0.3f
                    );

                if (t >= 1)
                {
                    // Swap & Update

                    var inventoryRef =
                        Globals.inventory.Value;

                    var dragItem =
                        inventoryRef[dragIdx];

                    var dropItem =
                        inventoryRef[dropIdx];

                    inventoryRef[dropIdx] = dragItem;
                    inventoryRef[dragIdx] = dropItem;

                    Globals.inventory.Value =
                        inventoryRef;

                    mouse.Value =
                        new None { };
                }
                else
                {
                    // Animate drop
                    dragView.transform.position =
                        Vector3.Lerp(
                            drop.offset,
                            icons[dropIdx].transform.position,
                            t
                        );

                    dropView.transform.position =
                        Vector3.Lerp(
                            icons[dropIdx].transform.position,
                            icons[dragIdx].transform.position,
                            t
                        );
                }
            });

        // Cancel

        mouse
            .AndThen(mouseValue =>
                Optional.FromCast<Base, Cancel>(mouseValue)
                    .Map(update.Always)
                    .WithDefault(Stream.None<Cancel>())
            )
            .Get(cancel =>
            {
                var t =
                    Functions.EaseInOut(
                        (Time.time - cancel.time) / 0.3f
                    );

                var index =
                    cancel.index;

                dragView.transform.position =
                    Vector3.Lerp(
                        cancel.offset,
                        icons[index].transform.position,
                        t
                    );

                if (t >= 1)
                {
                    mouse.Value =
                        new None { };
                }
            });

        // Hover Info effect

        var hoverInfo =
            Query
                .From(inventory, "hover-info")
                .Get();

        var hoverInfoTitle =
            Query
                .From(hoverInfo, "title")
                .Get<TMPro.TextMeshProUGUI>();

        var hoverInfoDescription =
            Query
                .From(hoverInfo, "description")
                .Get<TMPro.TextMeshProUGUI>();

        var hoverInfoComment =
            Query
                .From(hoverInfo, "comment")
                .Get<TMPro.TextMeshProUGUI>();

        hoverInfo.SetActive(false);

        mouse
            .Lazy()
            .Get(currentMouse =>
            {
                switch (
                    Optional
                        .FromCast<Base, Hover>(currentMouse)
                )
                {
                    case Some<Hover> hover:
                        var item =
                            Globals.inventory.Value[hover.Value.index];

                        var isEmpty =
                            Items.Empty.IsEmpty(item);

                        // Hover info

                        hoverInfo.SetActive(!isEmpty);

                        hoverInfoTitle.text = item.Title;
                        hoverInfoDescription.text = item.Description;
                        hoverInfoComment.text = item.Comment;

                        break;

                    default:
                        hoverInfo.SetActive(false);

                        break;
                }
            });

        // Drag image effect

        mouse
            .Get(currentMouse =>
            {
                Optional
                    .FromCast<Base, Drag>(currentMouse)
                    .Get(drag =>
                    {
                        dragView.SetActive(true);

                        dragViewImage.sprite =
                            itemsDictionary.SpriteFromItem(Globals.inventory.Value[drag.index]);
                    });

                Optional.FromCast<Base, None>(currentMouse)
                    .Get(_ =>
                    {
                        dragView.SetActive(false);
                    });
            });

        // change drag image position

        mouse
            .AndThen(currentMouse =>
                Optional
                    .FromCast<Base, Drag>(currentMouse)
                    .CaseOf(
                        update.Always,
                        Stream.None<Drag>
                    )
            )
            .Get(drag =>
            {
                dragView.transform.position =
                    Input.mousePosition - drag.offset;
            });

        // Play Sounds

        var cancelSound =
            Query
                .From(inventory, "sounds cancel")
                .Get<AudioSource>();

        var dropSound =
            Query
                .From(inventory, "sounds drop")
                .Get<AudioSource>();

        var dragSound =
            Query
                .From(inventory, "sounds drag")
                .Get<AudioSource>();

        mouse
            .Lazy()
            .WithLastValue(new None())
            .Get((lastMouse, currentMouse) =>
            {
                switch (currentMouse)
                {
                    case Cancel _:
                        cancelSound.Play();

                        break;

                    case Drop _:
                        dropSound.Play();
                        break;

                    case Drag _:
                        dragSound.Play();
                        break;
                }
            });

        // Drag Sword and Magic effect

        var swordSlotBlink =
            Query
                .From(inventory, "sword-slot-blink")
                .Get<Image>();

        var magicSlotBlink =
            Query
                .From(inventory, "magic-slot-blink")
                .Get<Image>();

        var extraSlot0Blink =
            Query
                .From(inventory, "extra-slot-0-blink")
                .Get<Image>();

        var extraSlot1Blink =
            Query
                .From(inventory, "extra-slot-1-blink")
                .Get<Image>();

        mouse
            .Initialized
            .Get(mouseValue =>
            {
                switch (mouseValue)
                {
                    case Drag drag:
                        var dragItem =
                            Globals.inventory.Value[drag.index];

                        var isSword =
                            Functions.IsTypeOf<Item, Items.Sword>(dragItem);

                        var isMagic =
                            Functions.IsTypeOf<Item, Items.Magic>(dragItem);

                        var isExtra =
                            !isSword && !isMagic && dragItem.CanEquip;

                        swordSlotBlink.gameObject.SetActive(isSword);
                        magicSlotBlink.gameObject.SetActive(isMagic);

                        extraSlot0Blink.gameObject.SetActive(isExtra);
                        extraSlot1Blink.gameObject.SetActive(isExtra);

                        break;

                    default:

                        swordSlotBlink.gameObject.SetActive(false);
                        magicSlotBlink.gameObject.SetActive(false);
                        extraSlot0Blink.gameObject.SetActive(false);
                        extraSlot1Blink.gameObject.SetActive(false);

                        break;
                }
            });

        mouse
            .AndThen(value =>
                Optional
                    .FromCast<Base, Drag>(value)
                    .CaseOf(update.Always, Stream.None<Drag>)
            )
            .Get(drag =>
            {
                var dragItem =
                    Globals.inventory.Value[drag.index];

                var blinkColor =
                    new Color(1.0f, 1.0f, 1.0f,
                        Mathf.Lerp(0.0f, 0.15f,
                            Functions.SinusoidalWave(Time.time - drag.time, 0.95f)
                        )
                    );


                if (Functions.IsTypeOf<Item, Items.Sword>(dragItem))
                {
                    swordSlotBlink.color =
                        blinkColor;
                }
                else if (Functions.IsTypeOf<Item, Items.Magic>(dragItem))
                {
                    magicSlotBlink.color =
                        blinkColor;
                }
                else if (dragItem.CanEquip)
                {
                    extraSlot0Blink.color =
                        blinkColor;
                    extraSlot1Blink.color =
                        blinkColor;
                }
            });
    }

    Optional<InventorySlot> HoveredInventorySlot(
        GraphicRaycaster graphicRaycaster,
        EventSystem eventSystem,
        Vector3 position)
    {
        foreach (var result in RaycastHelper.RaycastCanvas(graphicRaycaster, eventSystem, position))
        {
            if (result.CompareTag("InventorySlot"))
            {
                return
                    Optional.FromNullable(result.GetComponent<InventorySlot>());
            }
        }

        return Optional.None<InventorySlot>();
    }
}
