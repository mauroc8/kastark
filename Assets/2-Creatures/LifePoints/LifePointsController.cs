using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointsController : MonoBehaviour
{
    [SerializeField] CreatureController _creatureController = null;
    [SerializeField] GameObject _lifePointPrefab = null;

    List<LifePointBehaviour> _lifePoints;

    public List<LifePointBehaviour> LifePoints => _lifePoints;

    void Start()
    {
        var creature = _creatureController.creature;
        var lifePointsAmount = (int) creature.maxHealth;

        _lifePoints = new List<LifePointBehaviour>(lifePointsAmount);

        for (int i = 0; i < lifePointsAmount; i++)
        {
            var instance = Instantiate(_lifePointPrefab);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
            
            var lifePointBehaviour = instance.GetComponent<LifePointBehaviour>();
            _lifePoints.Add(lifePointBehaviour);

            lifePointBehaviour.percentage = (float) i / (float) lifePointsAmount;
        }
    }
}
