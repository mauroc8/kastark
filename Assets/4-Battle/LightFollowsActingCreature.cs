using Events;
using UnityEngine;

public class LightFollowsActingCreature : MonoBehaviour
{
    [SerializeField] float _speed = 1;

    CreatureController _actingCreature;

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
        _actingCreature = evt.actingCreature;
    }

    void Update()
    {
        if (_actingCreature == null) return;

        var pos = transform.position;
        var creaturePos = _actingCreature.head.position;
        creaturePos.y = pos.y;
        transform.position = Vector3.Lerp(pos, creaturePos, _speed * Time.deltaTime);
    }
}
