using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorTexture {
    Interactable, Friendly, Aggressive, Forbidden, None
};

public class CursorController : MonoBehaviour
{
    [SerializeField] Texture2D _interactableTexture;
    [SerializeField] Texture2D _friendlyTexture;
    [SerializeField] Texture2D _aggressiveTexture;
    [SerializeField] Texture2D _forbiddenTexture;

    static CursorController _instance;
    public static CursorController Instance {
        get {
            Debug.Assert(_instance != null);
            return _instance;
        }
    }

    void Awake() {
        Debug.Assert(_instance == null);
        _instance = this;
        DontDestroyOnLoad(this);
    }

    CursorTexture _currentTexture = CursorTexture.None;

    public void ChangeCursor(CursorTexture newTexture) {
        if (newTexture != _currentTexture) {
            _currentTexture = newTexture;
            switch (newTexture) {
                case CursorTexture.Interactable: {
                    Cursor.SetCursor(_interactableTexture, Vector2.zero, CursorMode.Auto);
                } break;
                case CursorTexture.Friendly: {
                    Cursor.SetCursor(_friendlyTexture, Vector2.zero, CursorMode.Auto);
                } break;
                case CursorTexture.Aggressive: {
                    Cursor.SetCursor(_aggressiveTexture, Vector2.zero, CursorMode.Auto);
                } break;
                case CursorTexture.Forbidden: {
                    Cursor.SetCursor(_forbiddenTexture, Vector2.zero, CursorMode.Auto);
                } break;
                case CursorTexture.None: {
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                } break;
            }
        }
    }
}
