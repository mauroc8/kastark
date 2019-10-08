using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLifePoints : MonoBehaviour
{
    [SerializeField] CreatureController _creatureController = null;
    [SerializeField] GameObject _lifePointsObject = null;
    [SerializeField] GameObject _lifePointPrefab = null;

    void Start()
    {
        var creature = _creatureController.creature;
        var lifePointsAmount = (int) creature.health;

        for (int i = 0; i < lifePointsAmount; i++)
        {
            var instance = Instantiate(_lifePointPrefab);
            instance.transform.SetParent(_lifePointsObject.transform);
        }
    }
}
