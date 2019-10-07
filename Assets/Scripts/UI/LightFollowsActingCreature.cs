using UnityEngine;

public class LightFollowsActingCreature : MonoBehaviour
{
    [SerializeField] float _speed = 1;

    void Update()
    {
        if (Global.actingCreature == null) return;

        var pos = transform.position;
        var creaturePos = Global.actingCreature.head.position;
        creaturePos.y = pos.y;
        transform.position = Vector3.Lerp(pos, creaturePos, _speed * Time.deltaTime);
    }
}
