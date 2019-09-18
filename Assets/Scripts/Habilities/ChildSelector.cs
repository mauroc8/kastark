using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class ChildSelector : MonoBehaviour
{
    GameObject[] _children;
    GameObject   _selected;

    void Start() {
        _children = Util.GetGameObjectChildrens(gameObject);

        foreach (var gameObject in _children) {
            gameObject.SetActive(false);
        }
    }

    public void SelectChild(int index) {
        if (_selected) _selected.SetActive(false);
        _selected = _children[index];
        _selected.SetActive(true);
    }
}
