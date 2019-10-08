using UnityEngine;

public class LifePointSpinningState : State
{
    public Transform transform;
    public float lifePointPercentage;

    public float spinSpeed;
    public float minHeight;
    public float maxHeight;
    public float spinRadius;
    public float movementSpeed;

    float _height;
    float _offset;

    public override void InitState() {
        _height = Mathf.Lerp(minHeight, maxHeight, lifePointPercentage);
        _offset = lifePointPercentage * Mathf.PI * 1234567;
    }

    public override void UpdateState(float dt) {
        var pos = transform.localPosition;
        var t = Time.time;
        var target = new Vector3(
            Mathf.Sin(_offset + t * spinSpeed) * spinRadius,
            _height,
            Mathf.Cos(_offset + t * spinSpeed) * spinRadius
        );
        transform.localPosition = Vector3.MoveTowards(pos, target, movementSpeed * dt);
    }

    public override void ExitState() {}
}
