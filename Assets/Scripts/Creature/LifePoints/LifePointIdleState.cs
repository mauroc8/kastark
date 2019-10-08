using UnityEngine;

public class LifePointIdleState : State
{
    public Transform transform;
    public FadeInController fadeInController;
    public FadeOutController fadeOutController;
    
    public override void InitState() {
        fadeOutController.FadeOut();
    }

    public override void UpdateState(float dt) { }

    public override void ExitState() {
        fadeInController.FadeIn();
    }
}
