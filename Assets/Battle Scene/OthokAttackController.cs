using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OthokAttackController : MonoBehaviour
{
    [SerializeField] UnityEvent _castEndEvent;

    void OnEnable()
    {
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(2);
        _castEndEvent.Invoke();
    }
}
