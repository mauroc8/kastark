using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sample
{
    public DateTime dateTime = DateTime.Now;
}

[Serializable]
public class LanguageSample : Sample
{
    public Language language;
}

[Serializable]
public class TeamSample : Sample
{
    public Team team;
}

[Serializable]
public class HabilityCastSample : Sample
{
    public DamageType damageType;
    public float baseDamage;
    public float difficulty;
    public float effectiveness;
    public string targetName;
}
