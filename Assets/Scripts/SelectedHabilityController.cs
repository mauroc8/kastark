using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;

public class SelectedHabilityController : MonoBehaviour
{
    GameObject[] _habilityGameObjects;

    [SerializeField]
    HabilityDescription[] _habilityDescriptions = new HabilityDescription[0];

    void Start() {
        _habilityGameObjects = gameObject.GetComponentsInChildren<GameObject>();
    }

    public void SelectHability(int habilityId) {
        CancelHability();
        _habilityGameObjects[(int) habilityId].SetActive(true);

        bool descriptionFound = false;
        foreach (var description in _habilityDescriptions) {
            if (habilityId == (int) description.habilityId) {
                EventController.TriggerEvent(new SelectedHabilityEvent{ habilityDescription = description });
                descriptionFound = true;
                break;
            }
        }

        Debug.Assert(descriptionFound);
    }

    public void CancelHability() {
        foreach (var habilityGO in _habilityGameObjects) {
            habilityGO.SetActive(false);
        }
    }
}
