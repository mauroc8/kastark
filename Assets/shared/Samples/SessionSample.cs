using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionSample : Sample
{
    public int sessionId = -1;

    public List<LanguageSample> chosenLanguages = new List<LanguageSample>(1);
    public List<HabilityCastSample> castHabilities = new List<HabilityCastSample>(10);
    public List<TeamSample> battleResults = new List<TeamSample>(2);
}
