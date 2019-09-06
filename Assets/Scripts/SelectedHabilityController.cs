using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedHabilityController : MonoBehaviour
{
    GameObject[] _habilityGameObjects;

    void Start() {
        _habilityGameObjects = Util.GetGameObjectChildrens(gameObject);
    }

    public void SelectHability(int habilityId) {
        CancelHability();
        _habilityGameObjects[(int) habilityId].SetActive(true);
    }

    public void CancelHability() {
        foreach (var habilityGO in _habilityGameObjects) {
            habilityGO.SetActive(false);
        }
    }
}
