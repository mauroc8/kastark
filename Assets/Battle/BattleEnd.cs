using UnityEngine;

public class BattleEnd : MonoBehaviour
{
    [Header("End battle")]
    [SerializeField] GameObject _battleWinNode;
    [SerializeField] GameObject _battleLoseNode;

    public void PlayerWins()
    {
        _battleWinNode.SetActive(true);
    }

    public void PlayerLooses()
    {
        _battleLoseNode.SetActive(true);
    }
}