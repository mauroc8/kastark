using UnityEngine;

public class LifePointSpinState : State
{
    public LifePointMovementController lifePointMovementController;

    public float lifePointPercentage;
    public float spinSpeed;
    public float spinRadius;
    public float minHeight;
    public float maxHeight;
    public float amountOfTurns;

    float _height;
    float _offset;

    public override void InitState() {
        _height = Mathf.Lerp(minHeight, maxHeight, lifePointPercentage);
        _offset = lifePointPercentage * Mathf.PI * 2 * amountOfTurns;
    }

    public override void UpdateState(float dt) {
        var t = Time.time;
        var target = new Vector3(
            Mathf.Sin(_offset + t * spinSpeed) * spinRadius,
            _height,
            Mathf.Cos(_offset + t * spinSpeed) * spinRadius
        );

        lifePointMovementController.MoveTowards(target);
    }

    public override void ExitState() {}
}
