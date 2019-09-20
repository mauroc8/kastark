﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Util
{
    public static GameObject[] GetGameObjectChildrens(GameObject root) {
        GameObject[] children = new GameObject[root.transform.childCount];

        int i = 0;
        foreach (Transform child in root.transform) {
            children[i++] = child.gameObject;
        }

        return children;
    }

    public static bool MouseIsOnUI() {
        // https://answers.unity.com/questions/844158/how-do-you-perform-a-graphic-raycast.html

        PointerEventData cursor = new PointerEventData(EventSystem.current);
        cursor.position = Input.mousePosition;
        List<RaycastResult> objectsHit = new List<RaycastResult> ();
        EventSystem.current.RaycastAll(cursor, objectsHit);

        foreach (RaycastResult result in objectsHit) {
            if (result.gameObject.CompareTag("UI")) {
                return true;
            }
        }

        return false;
    }

    public static GameObject GetHoveredGameObject()
    {
        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        
        if (Physics.Raycast(mRay, out hit)){
            return hit.transform.gameObject;
        }

        return null;
    }

    public static float FloatToHDCoords(float value) {
        return value * 1080f / Camera.main.pixelHeight;
    }

    public static Vector2 ScreenPointToHDCoords(Vector2 screenPoint) {
        return screenPoint * 1080f / Camera.main.pixelHeight;
    }
}
