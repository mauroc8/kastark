using UnityEngine;

public class UI_InteractableCursorOnHover : MonoBehaviour
{
    public void OnHover() {
        CursorController.ChangeCursorTexture(CursorTexture.Interactable);
    }
    public void OnBlur() {
        CursorController.ChangeCursorTexture(CursorTexture.None);
    }
}