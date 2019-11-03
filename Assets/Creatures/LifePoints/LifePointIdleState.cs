using UnityEngine;

public class LifePointIdleState : MonoBehaviour
{
    [SerializeField] LifePointManager _lifePointsManager = null;

    void OnEnable()
    {
        foreach (var lifePointController in _lifePointsManager.LifePointControllers)
        {
            lifePointController.alphaController.FadeOut(0.3f, 2);
        }
    }

    void OnDisable()
    {
        foreach (var lifePointController in _lifePointsManager.LifePointControllers)
        {
            lifePointController.alphaController.FadeIn(0.3f, 0.5f);
        }
    }
}
