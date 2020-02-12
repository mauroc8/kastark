
// Nota: Este diseño se abandonó.

/* Actualmente, programar en Unity significa crear "Componentes" que tienen acceso a
 * los "eventos" del ciclo de vida del componente, por ejemplo:
 *   - OnEnable
 *   - Update
 *   - OnDisable
 */

using System;
using UnityEngine;
using GlobalEvents;

class Hooks_v0 : MonoBehaviour
{
    void OnEnable()
    {
        // ..
    }

    void Update()
    {
        // ..
    }

    void OnDisable()
    {
        // ..
    }
}


/* Podríamos *usar* a Update adentro de OnEnable.
 * Por ejemplo, podríamos querer hacer diferentes cosas en Update
 * según una condición.
 */

class Hooks_v1 : MonoBehaviour
{
    Action _Update;

    void Update()
    {
        _Update();
    }


    bool _someCondition = false;

    void OnEnable()
    {
        if (_someCondition)
            _Update = () =>
            {
                // A
            };
        else
            _Update = () =>
            {
                // B
            };
    }
}

/* Podemos decidir qué hacer OnDisable en diferentes lugares.
 */

class Hooks_v2 : MonoBehaviour
{
    Action _OnDisable;

    void OnDisable()
    {
        _OnDisable();
    }


    // Por ejemplo, queremos suscribirnos a un evento.

    class MyEvent : GlobalEvent { }

    void MyEventListener(MyEvent a)
    {
        // ..
    }


    void OnEnable_v0()
    {
        EventController.AddListener<MyEvent>(MyEventListener);

        _OnDisable = () =>
        {
            EventController.RemoveListener<MyEvent>(MyEventListener);
        };
    }


    // Supongamos que ahora queremos suscribirnos a otro evento.

    class MyOtherEvent : GlobalEvent { }

    // Podemos evitar repetir código usando una función.

    void OnEnable_v1()
    {
        _OnDisable = () => { };

        UseEventListener<MyEvent>(MyEventListener);

        UseEventListener<MyOtherEvent>(otherEvent =>
        {
            // :O    ¡Incluso podemos usar lambdas!
        });
    }

    // ¿Qué hace esta función?

    void UseEventListener<Evt>(Action<Evt> listener) where Evt : GlobalEvent
    {
        EventController.AddListener<Evt>(listener);

        _OnDisable += () =>
        {
            EventController.RemoveListener<Evt>(listener);
        };
    }

    // Agregamos a OnDisable comportamiento a medida que lo vamos necesitando.
}


/* Estos son los *Hooks* básicos. Nos permiten "engancharnos" al ciclo de vida del componente.
 */

abstract class Hooks_v3 : MonoBehaviour
{
    protected abstract void Behaviour();

    void OnEnable()
    {
        _Update = () => { };
        _Disable = () => { };

        Behaviour();
    }


    Action _Update;
    Action _Disable;

    void Update()
    {
        _Update();
    }

    void OnDisable()
    {
        _Disable();
    }


    protected Action UseUpdate(Action update)
    {
        _Update += update;

        return () =>
        {
            _Update -= update;
        };
    }

    protected Action UseDisable(Action disable)
    {
        _Disable += disable;

        return () =>
        {
            _Disable -= disable;
        };
    }


    //   Combinando estos bloques básicos, se pueden hacer cosas muy interesantes.

    // La lógica contenida en un MonoBehaviour se puede escribir en *una sóla función*.

    // Significa que se puede *componer*, se puede llamar muchas veces, y se puede usar

    // para definir otras funciones. Y así...



}
