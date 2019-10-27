using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackController : MonoBehaviour
{
    [SerializeField] GameObject _attackTrailGameObject;
    [SerializeField] CountdownController _uiRectCountdown;
    [SerializeField] UnityEvent _castEndEvent;

    [Header("Countdown time")]
    [SerializeField] float _countdownTime = 2;

    AttackTrail _attackTrail;

    public void StopCasting()
    {
        StopAllCoroutines();
        _castEndEvent.Invoke();
    }

    IEnumerator Start()
    {
        _uiRectCountdown.StartCountdown(_countdownTime);

        while (true)
        {
            var newAttackTrailGo = Instantiate(_attackTrailGameObject);
            var attackTrail = newAttackTrailGo.GetComponent<AttackTrail>();

            yield return new WaitWhile(() => !Input.GetMouseButtonDown(0));

            if (!RaycastHelper.MouseIsOnUI())
            {
                attackTrail.Open(Input.mousePosition);

                while (Input.GetMouseButton(0))
                {
                    attackTrail.Move(Input.mousePosition);

                    if (attackTrail.IsOutOfBounds())
                        break;
                    
                    yield return null;
                }
            }

            attackTrail.Close();

            yield return null;
        }
    }
}
