using System.Collections;
using GlobalEvents;
using UnityEngine;

public class LightFollowsActingCreature : MonoBehaviour
{
    [SerializeField] float _duration = 0.4f;
    [SerializeField] float _power = 0.5f;

    void OnEnable()
    {
        EventController.AddListener<TurnStartEvent>(OnTurnStart);
    }

    void OnDisable()
    {
        EventController.RemoveListener<TurnStartEvent>(OnTurnStart);
    }

    void OnTurnStart(TurnStartEvent evt)
    {
        StartCoroutine(TurnStartCoroutine(evt.creature.transform));
    }

    public IEnumerator TurnStartCoroutine(Transform creatureTransform)
    {
        var time = Time.time;

        var initialPosition = transform.position;
        var creaturePosition = creatureTransform.position;

        creaturePosition.y = initialPosition.y;

        yield return null;

        while (Time.time < time + _duration)
        {
            var t = Mathf.Pow((Time.time - time) / _duration, _power);
            transform.position = Vector3.Lerp(initialPosition, creaturePosition, t);
            yield return null;
        }

        transform.position = creaturePosition;
    }
}
