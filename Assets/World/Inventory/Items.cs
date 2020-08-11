
public interface Item
{
    string Title { get; }
    string Description { get; }
    string Comment { get; }

    bool CanEquip { get; }
}

namespace Items
{
    public struct Empty : Item
    {
        public string Title => "";
        public string Description => "";
        public string Comment => "";

        public bool CanEquip => true;

        public static bool IsEmpty(Item item)
        {
            return Functions.IsTypeOf<Item, Items.Empty>(item);
        }
    }

    public struct SmallSword : Item
    {
        public string Title =>
            "Small Sword";

        public string Description =>
            "Cut your enemy 1 time.";

        public string Comment =>
            Sword.AbilityComment;

        public bool CanEquip => true;
    }

    public struct Sword : Item
    {
        public string Title =>
            "Sword";

        public string Description =>
            "Cut your enemy 3 times.";

        public string Comment =>
            Sword.AbilityComment;

        public static string AbilityComment =>
            "Drag and drop to the ability bar to equip.";

        public bool CanEquip => true;
    }

    public struct SmallMagic : Item
    {
        public string Title =>
            "Magic Beam (small)";

        public string Description =>
            "Cast 1 magical beam to hit enemies.";

        public string Comment =>
            Sword.AbilityComment;

        public bool CanEquip => true;
    }

    public struct Magic : Item
    {
        public string Title =>
            "Magic Beam";

        public string Description =>
            "Cast 3 magical beams to hit enemies.";

        public string Comment =>
            Sword.AbilityComment;

        public bool CanEquip => true;
    }

    public struct Shield : Item
    {
        public string Title =>
            "Shield (ability)";

        public string Description =>
            "Cast a shield to block enemy attacks.";

        public string Comment =>
            Sword.AbilityComment;

        public bool CanEquip => true;
    }

    public struct Potion : Item
    {
        public string Title =>
            $"Potion (x{amount})";

        public string Description =>
            "Restore 5 health points.";

        public string Comment =>
            Sword.AbilityComment;

        public bool CanEquip => true;

        public int amount;

        public Potion(int a)
        {
            amount = a;
        }
    }

    public struct Clover : Item
    {
        public string Title =>
            "Four-leaf Clover";

        public string Description =>
            "Return this clover to Dhénde.";

        public string Comment =>
            "";

        public bool CanEquip => false;
    }
}
