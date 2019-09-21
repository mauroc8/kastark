using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sample
{
    public DateTime dateTime = DateTime.Now;

    protected virtual bool Equals(Sample other)
    {
        return other.dateTime.Ticks == dateTime.Ticks;
    }
}

public class LanguageSample : Sample
{
    public Language language;
}

public class TeamSample : Sample
{
    public Team team;
}

public class HabilityCastSample : Sample
{
    public DamageType damageType;
    public float baseDamage;
    public float difficulty;
    public float effectiveness;
    public string targetName;
}
