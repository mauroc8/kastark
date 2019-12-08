using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyPart
{
    Head, Feet, None
}

public enum TeamId
{
    Player,
    Enemy,
    None
}

public enum DamageType
{
    Physical, Magical, Heal, Shield, None
}

public enum CreatureKind
{
    Fairy, Gnome, Dwarf, None
}

public static class LayerMask
{
    public static int Default = 0;
    public static int HabilityRaycast = 1 << 8;
}

