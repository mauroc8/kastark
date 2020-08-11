using UnityEngine;
using Vector3Extensions;
using System.Collections.Generic;
using System.Linq;
using System;

public class Battle : UpdateAsStream
{
    public StateStream<BattleStatus> status =
        new StateStream<BattleStatus>(BattleStatus.Preparing);

    Stream<Optional<Turn>> _turn = null;

    public Stream<Optional<Turn>> turn =>
        _turn != null
            ? _turn
            : (_turn =
                status
                    .Map(status =>
                        status.tag == BattleStatusTag.Fighting
                            ? Optional.Some(status.turn)
                            : Optional.None<Turn>()
                    )
              )
        ;

    void StartBattle()
    {
        status.Value =
            BattleStatus.Fighting;
    }

    Team playerTeam;
    Team enemyTeam;


    void Awake()
    {
        playerTeam =
            new Team(
                Query
                    .From(this, "aira")
                    .Get<Creature>()
            );

        var battleId =
            Scenes.battleId;

        enemyTeam =
            battleId == 0
                ? new Team(
                    Query
                        .From(this, "enemy-0")
                        .Get<Creature>()
                  )
                : battleId == 1
                ? new Team(
                    Query
                        .From(this, "enemy-1")
                        .Get<Creature>()
                  )
                : battleId == 2
                ? new Team(
                    Query
                        .From(this, "enemy-2")
                        .Get<Creature>()
                  )
                : battleId == 3
                ? new Team(
                    Query
                        .From(this, "enemy-3a")
                        .Get<Creature>(),
                    Query
                        .From(this, "enemy-3b")
                        .Get<Creature>()
                  )
                : new Team(
                    Query
                        .From(this, "enemy-4a")
                        .Get<Creature>(),
                    Query
                        .From(this, "enemy-4b")
                        .Get<Creature>()
                  )
            ;

        // Initialize enemy team

        var enemies =
            new Creature[]
            {
                Query.From(this, "enemy-0").Get<Creature>(),
                Query.From(this, "enemy-1").Get<Creature>(),
                Query.From(this, "enemy-2").Get<Creature>(),
                Query.From(this, "enemy-3a").Get<Creature>(),
                Query.From(this, "enemy-3b").Get<Creature>(),
                Query.From(this, "enemy-4a").Get<Creature>(),
                Query.From(this, "enemy-4b").Get<Creature>()
            };

        foreach (var enemy in enemies)
        {
            enemy.gameObject.SetActive(enemyTeam.creatures.Contains(enemy));
        }

        var currentTeam =
            turn
                .Map(optionalTurn => optionalTurn.Map(ToTeam))
                .Lazy();

        var currentCreature =
            turn
                .Map(value =>
                    value.Map(ToCreature)
                );

        // Animate camera and start battle

        var camera =
            Camera.main.transform;

        var initialPosition =
            camera.position;

        camera.Translate(
            Vector3.forward * 15, Space.Self
        );

        var battlePosition =
            camera.position;

        var firstTime =
            Functions.NewFirstTime();

        Stream
            .Of(Time.time)
            .AndThen(update.Always)
            .Get(initialTime =>
            {
                var dt =
                    Time.time - initialTime;

                var t =
                    Mathf.Pow(dt / 2.0f, 0.5f);

                camera.position =
                    Vector3.Lerp(
                        initialPosition,
                        battlePosition,
                        t
                    );

                if (t >= 1 && firstTime())
                {
                    StartBattle();
                }
            });

        // Move turnAction from Prepare to SelectAbility

        turn
            .FilterMap(a => a)
            .Filter(turnValue =>
                turnValue.action == TurnAction.Prepare
            )
            .AndThen(Functions.WaitForSeconds<Turn>(update, 0.8f))
            .Get(_ =>
            {
                status.Value =
                    status.Value
                        .WithTurn(
                            status.Value.turn
                                .WithAction(TurnAction.SelectAbility)
                        );
            });

        // When a creature has 0 lifepoints, it's enemy wins

        var creatures =
            GetComponentsInChildren<Creature>();

        Func<CreatureState, bool> creatureIsDead = state =>
            state.lifePoints
                .Where(lifePoint => lifePoint != LifePointState.Dead)
                .Count()
                == 0;

        foreach (var team in new[] { playerTeam, enemyTeam })
        {
            foreach (var creature in team.creatures)
            {
                creature.State
                    .Map(creatureIsDead)
                    .Lazy()
                    .Get(isDead =>
                    {
                        if (status.Value.tag == BattleStatusTag.Fighting)
                        {
                            ReportDeadCreature(creature);
                        }
                    });
            }
        }

        // End battle

        status
            .Filter(statusValue =>
                statusValue.tag == BattleStatusTag.PlayerWon
                    || statusValue.tag == BattleStatusTag.EnemyWon
            )
            .AndThen(Functions.WaitForSeconds<BattleStatus>(update, 1.2f))
            .Get(statusValue =>
            {
                Scenes.ReturnFromBattle(
                    statusValue.tag == BattleStatusTag.PlayerWon
                        ? BattleResult.Win
                        : BattleResult.Lose
                );
            });
    }

    private void ReportDeadCreature(Creature deadCreature)
    {
        playerTeam.creatures =
            playerTeam.creatures
                .Where(creature => creature != deadCreature)
                .ToArray();

        enemyTeam.creatures =
            enemyTeam.creatures
                .Where(creature => creature != deadCreature)
                .ToArray();

        if (playerTeam.creatures.Length == 0)
        {
            status.Value =
                status.Value.WithTag(BattleStatusTag.EnemyWon);
        }
        else if (enemyTeam.creatures.Length == 0)
        {
            status.Value =
                status.Value.WithTag(BattleStatusTag.PlayerWon);
        }
    }


    public Team ToTeam(Turn turn)
    {
        return
            ToTeam(turn.team);
    }

    public Creature ToCreature(Turn turn)
    {
        return
            ToTeam(turn.team).creatures[turn.creatureIndex];
    }


    public Team ToTeam(TeamId team)
    {
        return
            team == TeamId.Enemy ? enemyTeam : playerTeam;
    }

    public TeamId EnemyOf(TeamId team)
    {
        return
            team == TeamId.Enemy ? TeamId.Player : TeamId.Enemy;
    }

    public Creature ActingCreature(Turn turn)
    {
        return
            ToTeam(turn)
                .creatures[turn.creatureIndex];
    }

    public void CreatureSelectsAbility(Ability ability)
    {
        status.Value =
            status.Value
                .WithTurn(
                    status.Value.turn
                        .WithAction(TurnAction.CastAbility)
                        .WithSelectedAbility(ability)
                );
    }

    public void CreatureEndsTurn()
    {
        var turn =
            status.Value.turn;

        if (turn.creatureIndex + 1 >= ToTeam(turn).creatures.Length)
        {
            status.Value =
                status.Value
                    .WithTurn(
                        turn.IsEnemy
                            ? Turn.Player
                            : Turn.Enemy
                    );
        }
        else
        {
            turn.creatureIndex++;
            turn.action = TurnAction.Prepare;

            status.Value =
                status.Value
                    .WithTurn(
                        turn
                    );
        }
    }

}