using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LifePointManager : MonoBehaviour
{
    [SerializeField] Creature _creature;
    [SerializeField] GameObject _lifePoint;

    List<GameObject> _lifePoints = new List<GameObject>();

    public Creature Creature => _creature;
    public int MaxLifePoints => (int)_creature.maxHealth;
    public List<GameObject> LifePoints => _lifePoints;
    public List<LifePointController> LifePointControllers =>
        _lifePoints.ConvertAll(go => go.GetComponent<LifePointController>());

    void Awake()
    {
        var maxLifePoints = MaxLifePoints;

        for (int i = 0; i < maxLifePoints; i++)
        {
            var lifePoint = Instantiate(_lifePoint);
            lifePoint.SetActive(true);
            lifePoint.transform.SetParent(transform);
            lifePoint.transform.localPosition = Vector3.zero;

            _lifePoints.Add(lifePoint);
        }
    }

    public void OnLifePointHit(GameObject lifePoint)
    {
        _lifePoints.Remove(lifePoint);
        Destroy(lifePoint);
    }
}
