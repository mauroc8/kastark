using UnityEngine;

public class LifePointIdleState : MonoBehaviour
{
    [SerializeField] LifePointsController _lifePointsController = null;
    
    void OnEnable()
    {
        foreach (var lifePoint in _lifePointsController.LifePoints)
        {
            lifePoint.FadeOut();
        }
    }

    void OnDisable()
    {
        foreach (var lifePoint in _lifePointsController.LifePoints)
        {
            lifePoint.FadeIn();
        }
    }
}
