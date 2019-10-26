using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using UnityEngine.Events;

public class Battle : MonoBehaviour
{
    public Team playerTeam;
    public Team enemyTeam;

    [SerializeField]
    UnityEvent _battleWinEvent;

    [SerializeField]
    UnityEvent _battleLoseEvent;

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
        #pragma warning disable 168
        catch (TaskCanceledException e) {}
        #pragma warning restore 168
    }

    void OnDisable()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }

    async Task BattleStart(CancellationToken token)
    {
        var teams = new Team[]{ playerTeam, enemyTeam };

        while (true)
        {
            foreach (var team in teams)
            {
                foreach (var creature in team.Creatures)
                {
                    if (!playerTeam.IsAlive)
                    {
                        _battleLoseEvent.Invoke();
                        return;
                    }

                    if (!enemyTeam.IsAlive)
                    {
                        _battleWinEvent.Invoke();
                        return;
                    }
                    
                    token.ThrowIfCancellationRequested();

                    await creature.TurnAsync(token);
                }
            }
        }
    }
}
