﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointController : MonoBehaviour
{
    [SerializeField] Creature _creature = null;

    public void GetsHit()
    {
        _creature.controller.ReceiveAttack(1);
        Destroy(gameObject);
    }
}
