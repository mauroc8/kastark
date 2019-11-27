using UnityEngine;

public class LifePointSpinState : MonoBehaviour
{
    // For debugging
    [SerializeField] string Name;
    
    [Header("Refs")]
    [SerializeField] LifePointManager _lifePointsManager = null;

    [Header("Spin")]
    [SerializeField] float _spinSpeed = 1;
    [SerializeField] float _spinRadius = 1;
    [Header("Height")]
    [SerializeField] float _minHeight = 1;
    [SerializeField] float _maxHeight = 1;

    [Header("Turns")]
    [SerializeField] float _amountOfTurns = 1;

    void Update()
    {
        var spinSpeed = _spinSpeed / _spinRadius;
        var lifePoints = _lifePointsManager.LifePoints;
        var maxLifePoints = _lifePointsManager.MaxLifePoints;

        for (int i = 0; i < lifePoints.Count; i++)
        {
            var lifePoint = lifePoints[i];
            var percentage = (float)(i + 1) / (float)maxLifePoints;

            var t = Time.time;
            var offset = percentage * _amountOfTurns * Mathf.PI * 2;
            var height = Mathf.Lerp(_minHeight, _maxHeight, percentage);
            height *= _lifePointsManager.Creature.Height;

            var target = new Vector3(
                Mathf.Sin(offset + t * spinSpeed) * _spinRadius,
                height,
                Mathf.Cos(offset + t * spinSpeed) * _spinRadius
            );

            lifePoint.transform.localPosition = target;
        }
    }
}
