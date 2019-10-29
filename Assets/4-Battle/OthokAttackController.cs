using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class OthokAttackController : MonoBehaviour
{
    [SerializeField] UnityEvent _castEndEvent;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        _castEndEvent.Invoke();
    }
}
