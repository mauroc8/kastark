using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorTexture {
    Interactable, Friendly, Aggressive, Forbidden, None
};

[CreateAssetMenu(menuName="Kastark/Cursor Skin")]
public class CursorSkin : ScriptableObject
{
    [SerializeField] Texture2D _interactableTexture = null;
    [SerializeField] Texture2D _friendlyTexture = null;
    [SerializeField] Texture2D _aggressiveTexture = null;
    [SerializeField] Texture2D _forbiddenTexture = null;

    private CursorTexture _currentTexture = CursorTexture.None;

    public void ChangeCursorTexture(CursorTexture newTexture) {
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

    public void ChangeToInteractable() {
        ChangeCursorTexture(CursorTexture.Interactable);
    }
    public void ChangeToDefault() {
        ChangeCursorTexture(CursorTexture.None);
    }
}
