using UnityEngine;

public class LifePointStateManager : MonoBehaviourStateMachine
{
    [SerializeField] Event _enemySelectsHability;
    [SerializeField] Event _enemyEndsTurn;

    [SerializeField] MonoBehaviour _idleState;
    [SerializeField] MonoBehaviour _capsuleSpinState;
    [SerializeField] MonoBehaviour _beltSpinState;

    void OnEnable()
    {
        _enemySelectsHability.AddListener(OnEnemyHabilitySelect);
        _enemyEndsTurn.AddListener(OnEnemyTurnEnd);
    }

    void OnDisable()
    {
        _enemySelectsHability.RemoveListener(OnEnemyHabilitySelect);
        _enemyEndsTurn.RemoveListener(OnEnemyTurnEnd);
    }

    void Start()
    {
        SwitchState(_idleState);
    }

    public void OnEnemyHabilitySelect()
    {
        if (Random.Range(0, 100) < 50)
        {
            SwitchState(_capsuleSpinState);
        }
        else
        {
            SwitchState(_beltSpinState);
        }
    }

    public void OnEnemyTurnEnd()
    {
        SwitchState(_idleState);
    }
}
