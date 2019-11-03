using UnityEngine;

public class LifePointSpinState : MonoBehaviour
{
    [SerializeField] LifePointManager _lifePointsManager = null;

    [SerializeField] float _spinSpeed = 1;
    [SerializeField] float _spinRadius = 1;
    [SerializeField] float _minHeight = 1;
    [SerializeField] float _maxHeight = 1;
    [SerializeField] float _amountOfTurns = 1;

    void Update()
    {
        var spinSpeed = _spinSpeed / _spinRadius;
        var lifePoints = _lifePointsManager.LifePoints;

        for (int i = 0; i < lifePoints.Count; i++)
        {
            var lifePoint = lifePoints[i];
            var percentage = (float)i / (float)lifePoints.Count;

            var t = Time.time;
            var offset = percentage * _amountOfTurns * Mathf.PI * 2;
            var height = Mathf.Lerp(_minHeight, _maxHeight, percentage);

            var target = new Vector3(
                Mathf.Sin(offset + t * spinSpeed) * _spinRadius,
                height,
                Mathf.Cos(offset + t * spinSpeed) * _spinRadius
            );

            lifePoint.transform.localPosition = target;
        }
    }
}
