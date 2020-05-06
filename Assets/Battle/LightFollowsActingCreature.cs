using System.Collections;
using GlobalEvents;
using UnityEngine;

public class LightFollowsActingCreature : MonoBehaviour
{
    float _duration = 0.5f;
    float _power = 0.5f;

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
        transform.position = evt.creature.feet.position;
        //StartCoroutine(TurnStartCoroutine(evt.creature.feet));
    }

    public IEnumerator TurnStartCoroutine(Transform feet)
    {
        var time = Time.time;

        var initialPosition = transform.position;
        var creaturePosition = feet.position;

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
