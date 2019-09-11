using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class SelectedHabilityController : MonoBehaviour
{
    GameObject[] _habilityGameObjects;

    void Start() {
        _habilityGameObjects = Util.GetGameObjectChildrens(gameObject);
    }

    public void SelectHability(int habilityId) { // habilityId should be HabilityId but Unity won't show it when setting OnClick handlers
        CancelHability();
        _habilityGameObjects[(int) habilityId].SetActive(true);

        EventController.TriggerEvent(new SelectedHabilityEvent{ habilityId = (HabilityId) habilityId });
    }

    public void CancelHability() {
        foreach (var habilityGO in _habilityGameObjects) {
            habilityGO.SetActive(false);
        }
    }
}
