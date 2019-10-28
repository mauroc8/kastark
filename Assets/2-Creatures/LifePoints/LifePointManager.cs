using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LifePointManager : MonoBehaviour
{
    [SerializeField] Creature _creature;
    [SerializeField] GameObject _lifePoint;

    List<GameObject> _lifePoints = new List<GameObject>();

    public List<GameObject> LifePoints => _lifePoints;
    public List<LifePointController> LifePointControllers =>
        _lifePoints.ConvertAll(
            lifePoint => lifePoint.GetComponent<LifePointController>()
        );

    void Start()
    {
        var lifePointsAmount = (int)_creature.maxHealth;

        for (int i = 0; i < lifePointsAmount; i++)
        {
            var lifePoint = Instantiate(_lifePoint);
            lifePoint.SetActive(true);
            lifePoint.transform.SetParent(transform);
            lifePoint.transform.localPosition = Vector3.zero;

            var percentage = (float)i / (float)lifePointsAmount;

            _lifePoints.Add(lifePoint);
        }
    }

    public void OnLifePointHit(GameObject lifePoint)
    {
        _lifePoints.Remove(lifePoint);
        Destroy(lifePoint);
    }
}
