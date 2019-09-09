using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class UI_ScreensManager : MonoBehaviour
{
    GameObject[] _screens;
    int _currentScreenIndex;

    void Start() {
        _screens = gameObject.GetComponentsInChildren<GameObject>();
        _currentScreenIndex = 0;

        foreach (var screen in _screens) {
            screen.SetActive(false);
        }

        _screens[_currentScreenIndex].SetActive(true);
    }

    public void MoveScreen(int move) {
        _screens[_currentScreenIndex].SetActive(false);
        _currentScreenIndex = (_currentScreenIndex + move) % _screens.Length;
        _screens[_currentScreenIndex].SetActive(true);
    }

    void OnEnable() {
        EventController.AddListener<StartUnitTurnEvent>(OnStartUnitTurn);
    }

    void OnDisable() {
        EventController.RemoveListener<StartUnitTurnEvent>(OnStartUnitTurn);
    }

    void OnStartUnitTurn(StartUnitTurnEvent e) {
        MoveScreen(1);
    }
}
