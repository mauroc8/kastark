using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OthokAttackController : MonoBehaviour
{
    [SerializeField] Team _targetTeam;

    [SerializeField] GameObject _trail;
    [SerializeField] UnityEvent _castEndEvent;

    void OnEnable()
    {
        StartCoroutine(Attack());
    }

    [SerializeField] int _amountOfCuts = 4;

    float _cutLengthVh = 0.25f;
    float _cutDuration = 0.3f;

    IEnumerator Attack()
    {
        // Think before first cut
        yield return new WaitForSeconds(1.6f);

        var targetCreature = _targetTeam.Creatures[0];

        var targetLifePointManager = targetCreature.gameObject.GetComponentInChildren<LifePointManager>();

        var creature =
            GetComponentInParent<Creature>();

        var animator =
            creature.GetComponentInChildren<Animator>();

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
                    GetTargetPoint(targetPosition, pathStart, pathEnd, 0)
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
                    GetTargetPoint(targetLifePointPosition, pathStart, pathEnd, t)
                );

                yield return null;
            }

            attackTrail.Close();

            // Think after each cut
            yield return new WaitForSeconds(0.25f);
        }

        _castEndEvent.Invoke();
    }

    Vector2 GetTargetPoint(
        Vector3 lifePointPosition,
        Vector2 pathStart,
        Vector2 pathEnd,
        float t)
    {
        Vector2 targetPoint = Camera.main.WorldToScreenPoint(lifePointPosition);

        var pathOffset = Vector2.Lerp(pathStart, pathEnd, t);

        return
            targetPoint +
            pathOffset
        ;
    }
}
