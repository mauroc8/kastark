using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Events;

public class SelectHabilityUIController : MonoBehaviour
{
    [SerializeField]
    GameObject _selectHabilityScreen = null;
    [SerializeField]
    GameObject _habilityDescriptionScreen = null;

    HabilityDescriptionController _habilityDescriptionController;

    [SerializeField]
    HabilityInfo[] _habilitiesInfo = new HabilityInfo[3];

    void Start() {
        _habilityDescriptionController = _habilityDescriptionScreen.GetComponent<HabilityDescriptionController>();
        _selectHabilityScreen.SetActive(false);
        _habilityDescriptionScreen.SetActive(false);
    }

    void OnEnable() {
        EventController.AddListener<PlayerTurnBeginEvent>(OnPlayerTurnBegin);
    }
    void OnDisable() {
        EventController.RemoveListener<PlayerTurnBeginEvent>(OnPlayerTurnBegin);
    }

    void OnPlayerTurnBegin(PlayerTurnBeginEvent e) {
        _selectHabilityScreen.SetActive(true);
    }

    HabilityInfo _selectedHability;

    public void SelectHability(int i) {
        _selectHabilityScreen.SetActive(false);

        _selectedHability = _habilitiesInfo[i];
        _habilityDescriptionController.SetDescription(_selectedHability);

        _habilityDescriptionScreen.SetActive(true);
        
        EventController.TriggerEvent(new HabilitySelectEvent{ habilityInfo = _selectedHability });
    }

    bool _selectedHabilityIsLocked = false;

    public void CancelSelectedHability() {
        if (_selectedHabilityIsLocked) {
            // Error sound.
            return;
        }
        _habilityDescriptionScreen.SetActive(false);

        _selectHabilityScreen.SetActive(true);
    }

    public HabilityInfo GetSelectedHabilityInfo() {
        return _selectedHability;
    }

    public void LockSelectedHability() {
        _selectedHabilityIsLocked = true;
        // Disable button, probably.
    }
}
