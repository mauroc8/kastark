using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public Team playerTeam;
    public Team enemyTeam;

    public BattleEnd _battleEnd = null;

    CancellationTokenSource _cancellationTokenSource;

    async void OnEnable()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await BattleStart(
                _cancellationTokenSource.Token
            );
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        catch (TaskCanceledException e)
        {
            // Evitamos un warning ... :facepalm:
            if (e.GetType() == typeof(TaskCanceledException))
            {
            }
        }
    }

    void OnDisable()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    async Task BattleStart(CancellationToken token)
    {
        var teams = new Team[] { playerTeam, enemyTeam };

        while (true)
        {
            foreach (var team in teams)
            {
                foreach (var creature in team.Creatures)
                {
                    token.ThrowIfCancellationRequested();

                    await creature.Turn.TurnAsync(token);

                    if (BattleWinStatus.battleEnded)
                    {
                        StartCoroutine(BattleEndCoroutine(BattleWinStatus.looser));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator BattleEndCoroutine(Creature looser)
    {
        if (_battleEnd != null)
        {
            if (looser.name == "Aira")
                _battleEnd.PlayerLooses();
            else
                _battleEnd.PlayerWins();
        }

        var animator =
            looser.gameObject.GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("is_dead", true);
        }

        yield return new WaitForSeconds(1.5f);

        if (looser.name == "Aira")
            WorldBattleCommunication.AiraLostBattle();
        else
            WorldBattleCommunication.OthokLostBattle();


        if (animator != null)
        {
            animator.SetBool("is_dead", false);
        }
    }
}
