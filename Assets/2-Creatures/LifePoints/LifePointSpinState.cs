using UnityEngine;

public class LifePointSpinState : MonoBehaviour
{
    [SerializeField] LifePointsController _lifePointsController = null;

    [SerializeField] float _spinSpeed = 1;
    [SerializeField] float _spinRadius = 1;
    [SerializeField] float _minHeight = 1;
    [SerializeField] float _maxHeight = 1;
    [SerializeField] float _amountOfTurns = 1;

    void Update() {
        var spinSpeed = _spinSpeed / _spinRadius;
        foreach (var lifePointBehaviour in _lifePointsController.LifePoints)
        {
            if (lifePointBehaviour == null) continue; // Destroyed instances
            
            var t = Time.time;
            var offset = lifePointBehaviour.percentage * _amountOfTurns * Mathf.PI * 2;
            var height = Mathf.Lerp(_minHeight, _maxHeight, lifePointBehaviour.percentage);
            
            var target = new Vector3(
                Mathf.Sin(offset + t * spinSpeed) * _spinRadius,
                height,
                Mathf.Cos(offset + t * spinSpeed) * _spinRadius
            );

            lifePointBehaviour.SetTarget(target);
        }
    }
}
