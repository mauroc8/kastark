using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePointManager : MonoBehaviour
{
    [SerializeField] Creature _creature;
    [SerializeField] GameObject _lifePointPrefab;

    List<LifePointController> _lifePoints;

    public List<LifePointController> LifePoints => _lifePoints;

    void Start()
    {
        var lifePointsAmount = (int) _creature.maxHealth;

        _lifePoints = new List<LifePointController>(lifePointsAmount);

        for (int i = 0; i < lifePointsAmount; i++)
        {
            var instance = Instantiate(_lifePointPrefab);
            instance.transform.SetParent(transform);
            instance.transform.localPosition = Vector3.zero;
            
            var lifePointBehaviour = instance.GetComponent<LifePointController>();
            _lifePoints.Add(lifePointBehaviour);

            lifePointBehaviour.percentage = (float) i / (float) lifePointsAmount;
            // this is basically for suscribing to messages, like GotHit
            lifePointBehaviour.lifePointManager = this;
        }
    }

    public void LifePointGotHit(LifePointController lifePoint)
    {
        _lifePoints.Remove(lifePoint);

        Destroy(lifePoint.gameObject);
    }
}
