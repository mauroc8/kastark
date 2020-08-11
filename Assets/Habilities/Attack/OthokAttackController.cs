using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OthokAttackController : Ability
{
    public override bool IsAgressive =>
        true;

    void Awake()
    {
        var creature =
            GetComponentInParent<Creature>();

        var battle =
            GetComponentInParent<Battle>();

        var isCasting =
            battle
                .turn
                .Map(optionalTurn =>
                    optionalTurn.CaseOf(
                        turn =>
                            battle.ActingCreature(turn) == creature
                                && turn.action == TurnAction.CastAbility
                                && turn.selectedAbility == this,
                        () => false
                    )
                )
                .Lazy();

        var castStart =
            isCasting
                .Filter(a => a);

        castStart
            .Get(_ =>
            {
                StartCoroutine(Attack(battle, creature));
            });
    }

    [SerializeField] GameObject _trail;

    [SerializeField] int _amountOfCuts = 4;

    float _cutLengthVh = 0.25f;
    float _cutDuration = 0.3f;

    IEnumerator Attack(Battle battle, Creature creature)
    {
        // Think before first cut
        yield return new WaitForSeconds(1.6f);

        var targetCreature =
            Query
                .From(transform.root, "aira")
                .Get<Creature>();

        var targetLifePointManager =
            targetCreature.gameObject.GetComponentInChildren<LifePointManager>();

        var animator =
            Query
                .From(creature, "mesh")
                .Get<Animator>();

        Debug.Assert(targetLifePointManager != null);

        var centerPosition =
            Vector3.Lerp(targetCreature.head.position, targetCreature.feet.position, 0.5f);

        var swooshSource =
            GetComponent<AudioSource>();

        // Play animation

        if (animator != null)
        {
            animator.SetTrigger("attack_sword");
        }

        for (int i = 0; i < _amountOfCuts; i++)
        {
            // Pick a target life point
            var targetLifePoints = targetLifePointManager.LifePoints;
            var targetLifePoint = targetLifePoints[Random.Range(0, targetLifePoints.Count - 1)];
            var targetLifePointPosition = targetLifePoint.transform.position;

            var targetPosition =
                Vector3.Lerp(centerPosition, targetLifePointPosition, 0.6f);

            var targetPosition2 =
                Camera.main.WorldToScreenPoint(targetPosition);

            // Pick a short path centered on (0, 0)
            var angle = Random.Range(-Mathf.PI / 6, Mathf.PI / 6);

            var cutRadius = _cutLengthVh / 2 * Screen.height;
            var pathEnd = new Vector2(
                Mathf.Cos(angle) * cutRadius,
                Mathf.Sin(angle) * cutRadius
            );
            var pathStart = new Vector2(
                Mathf.Cos(angle + Mathf.PI) * cutRadius,
                Mathf.Sin(angle + Mathf.PI) * cutRadius
            );

            yield return null;

            // Create and open the trail
            var startTime = Time.time;
            var attackTrail = Instantiate(_trail).GetComponent<AttackTrail>();
            attackTrail.Open(
                    GetTargetPoint(targetPosition2, pathStart, pathEnd, 0)
            );

            // Play swoosh sound

            if (swooshSource != null)
            {
                Functions.PlaySwooshSound(swooshSource);
            }

            yield return null;

            while (Time.time - startTime < _cutDuration)
            {
                var t = (Time.time - startTime) / _cutDuration;
                attackTrail.Move(
                    GetTargetPoint(targetPosition2, pathStart, pathEnd, t)
                );

                yield return null;
            }

            attackTrail.Close();

            // Think after each cut
            yield return new WaitForSeconds(0.25f);
        }

        battle
            .CreatureEndsTurn();
    }

    Vector2 GetTargetPoint(
        Vector2 targetPoint,
        Vector2 pathStart,
        Vector2 pathEnd,
        float t)
    {
        var pathOffset = Vector2.Lerp(Vector2.zero, pathEnd - pathStart, t);

        return
            targetPoint +
            pathOffset
        ;
    }
}
