using System.Collections;
using GlobalEvents;
using UnityEngine;

public class LightFollowsActingCreature : UpdateAsStream
{
    void Awake()
    {
        var battle =
            GetComponentInParent<Battle>();

        var changeTurnSound =
            Query
                .From(this, "change-turn-sound")
                .Get<AudioSource>();

        battle
            .turn
            .Map(maybeTurn =>
                maybeTurn.Map(battle.ActingCreature)
            )
            .Lazy()
            .AndThen(Functions.WaitForSeconds<Optional<Creature>>(update, 0.4f))
            .Get(maybeCreature =>
            {
                switch (maybeCreature)
                {
                    case Some<Creature> c:

                        transform.position =
                            c.Value.feet.position;

                        changeTurnSound.Play();

                        break;

                    case None<Creature> _:

                        transform.position =
                            new Vector3(0.0f, -99.0f, 0.0f);

                        break;
                }

            });

        battle
            .status
            .Map(status => status.tag)
            .Filter(tag => tag != BattleStatusTag.Fighting)
            .InitializeWith(BattleStatusTag.Preparing)
            .Get(_ =>
            {
                transform.position =
                    new Vector3(-100, -100, -100);
            });
    }
}
