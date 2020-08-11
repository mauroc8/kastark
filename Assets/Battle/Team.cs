using System.Collections.Generic;

public class Team
{
    public Creature[] creatures;

    public Team(params Creature[] creatures)
    {
        this.creatures = creatures;
    }
}