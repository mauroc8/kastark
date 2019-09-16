using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorTexture {
    Interactable, Friendly, Aggressive, Forbidden, None
};

public class CursorController : MonoBehaviour
{
    [SerializeField] Texture2D _interactableTexture = null;
    [SerializeField] Texture2D _friendlyTexture = null;
    [SerializeField] Texture2D _aggressiveTexture = null;
    [SerializeField] Texture2D _forbiddenTexture = null;

    private static CursorController _instance;
    public static CursorController Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<CursorController>() as CursorController;
                Debug.Assert(_instance);
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    private CursorTexture _currentTexture = CursorTexture.None;

    public static void ChangeCursorTexture(CursorTexture newTexture) {
        if (newTexture != Instance._currentTexture) {
            Instance._currentTexture = newTexture;
            switch (newTexture) {
                case CursorTexture.Interactable: {
                    Cursor.SetCursor(Instance._interactableTexture, Vector2.zero, CursorMode.Auto);
                } break;
                case CursorTexture.Friendly: {
                    Cursor.SetCursor(Instance._friendlyTexture, Vector2.zero, CursorMode.Auto);
                } break;
                case CursorTexture.Aggressive: {
                    Cursor.SetCursor(Instance._aggressiveTexture, Vector2.zero, CursorMode.Auto);
                } break;
                case CursorTexture.Forbidden: {
                    Cursor.SetCursor(Instance._forbiddenTexture, Vector2.zero, CursorMode.Auto);
                } break;
                case CursorTexture.None: {
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                } break;
            }
        }
    }

    public void ChangeToInteractable() {
        ChangeCursorTexture(CursorTexture.Interactable);
    }
    public void ChangeToDefault() {
        ChangeCursorTexture(CursorTexture.None);
    }
}
