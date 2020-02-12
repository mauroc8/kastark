using System.Collections.Generic;
using UnityEngine;

public class LifePointManager : StreamBehaviour
{
    [SerializeField] GameObject _lifePoint;

    List<GameObject> _lifePoints = new List<GameObject>();

    public List<GameObject> LifePoints => _lifePoints;

    public List<LifePointController> LifePointControllers =>
        _lifePoints.ConvertAll(lp => lp.GetComponent<LifePointController>());

    protected override void Awake()
    {
        var creature = GetContext<Creature>();


        for (int i = 0; i < creature.maxHealth; i++)
        {
            var lifePoint = Instantiate(_lifePoint);
            lifePoint.transform.SetParent(transform);
            lifePoint.transform.localPosition = Vector3.zero;

            lifePoint.GetComponent<LifePointController>().Init(i);
            lifePoint.SetActive(true);

            _lifePoints.Add(lifePoint);
        }
    }
}
