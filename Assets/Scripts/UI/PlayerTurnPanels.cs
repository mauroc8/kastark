using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class PlayerTurnPanels : MonoBehaviour
{
    [SerializeField] GameObject[] _screens = {};
    GameObject _currentScreen;

    void SelectScreen(int index)
    {
        if (_currentScreen != null)
        {
            _currentScreen.SetActive(false);
        }
        _currentScreen = _screens[index];
        if (_currentScreen != null)
        {
            _currentScreen.SetActive(true);
        }
    }

    void OnEnable() {
        EventController.AddListener<StartCreatureTurnEvent>(OnStartUnitTurn);
        EventController.AddListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.AddListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnDisable() {
        EventController.RemoveListener<StartCreatureTurnEvent>(OnStartUnitTurn);
        EventController.RemoveListener<HabilitySelectEvent>(OnHabilitySelect);
        EventController.RemoveListener<HabilityCastEvent>(OnHabilityCast);
    }

    void OnStartUnitTurn(StartCreatureTurnEvent evt)
    {
        SelectScreen(0);
    }

    GameObject _instance;

    void OnHabilitySelect(HabilitySelectEvent evt)
    {
        Debug.Log($"HabilitySelected:{evt.hability.Name}");
        SelectScreen(1);
        _instance = Instantiate(evt.hability.Controller);
        _instance.transform.SetParent(transform, false);
    }

    void OnHabilityCast(HabilityCastEvent evt)
    {
        //Destroy(_instance);
    }
}
