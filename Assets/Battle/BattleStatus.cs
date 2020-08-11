
public enum TurnAction
{
    Prepare,
    SelectAbility,
    CastAbility
};

public struct Turn
{
    public TeamId team;
    public int creatureIndex;
    public TurnAction action;
    public Ability selectedAbility;

    public bool IsPlayer =>
        team == TeamId.Player;

    public bool IsEnemy =>
        team == TeamId.Enemy;

    public Turn WithAction(TurnAction action)
    {
        return new Turn
        {
            team = this.team,
            creatureIndex = this.creatureIndex,
            action = action,
            selectedAbility = this.selectedAbility
        };
    }

    public Turn WithSelectedAbility(Ability selectedAbility)
    {
        return new Turn
        {
            team = this.team,
            creatureIndex = this.creatureIndex,
            action = this.action,
            selectedAbility = selectedAbility
        };
    }

    public static Turn Player =>
        new Turn
        {
            team = TeamId.Player,
            creatureIndex = 0,
            action = TurnAction.Prepare
        };

    public static Turn Enemy =>
        new Turn
        {
            team = TeamId.Enemy,
            creatureIndex = 0,
            action = TurnAction.Prepare
        };
}

public enum BattleStatusTag
{
    Preparing,
    Fighting,
    PlayerWon,
    EnemyWon
};

public struct BattleStatus
{
    public BattleStatusTag tag;
    public Turn turn;

    public static BattleStatus Preparing =>
        new BattleStatus { tag = BattleStatusTag.Preparing, turn = Turn.Player };

    public static BattleStatus Fighting =>
        new BattleStatus { tag = BattleStatusTag.Fighting, turn = Turn.Player };

    public BattleStatus WithTurn(Turn turn)
    {
        return new BattleStatus
        {
            tag = this.tag,
            turn = turn
        };
    }

    public BattleStatus WithTag(BattleStatusTag tag)
    {
        return new BattleStatus
        {
            tag = tag,
            turn = this.turn
        };
    }
}