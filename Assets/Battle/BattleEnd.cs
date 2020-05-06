using UnityEngine;

public class BattleEnd : MonoBehaviour
{
    [Header("End battle")]
    [SerializeField] GameObject _battleWinNode;
    [SerializeField] GameObject _battleLoseNode;

    Clover _clover = null;

    public void PlayerWins()
    {
        _battleWinNode.SetActive(true);

        // Show clover

        if (_clover == null)
            _clover =
                Query
                    .From(transform.root, "clover")
                    .Get<Clover>();

        _clover.PlayerWins();
    }

    public void ReturnToMainMenu()
    {
        Globals.SetScene(1);
    }

    public void ReturnToWorld()
    {
        Globals.SetScene(2);
    }

    public void PlayerLooses()
    {
        _battleLoseNode.SetActive(true);
    }
}