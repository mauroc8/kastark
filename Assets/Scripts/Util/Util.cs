﻿using System.Collections;
using System.Collections.Generic;
using Events;
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

    public static GameObject GetGameObjectAtScreenPoint(Vector2 screenPoint)
    {
        Ray mRay = Camera.main.ScreenPointToRay(screenPoint);

        RaycastHit hit;
        
        if (Physics.Raycast(mRay, out hit)){
            return hit.transform.gameObject;
        }

        return null;
    }

    public static GameObject GetHoveredGameObject()
    {
        return GetGameObjectAtScreenPoint(Input.mousePosition);
    }

    public static float FloatToHDCoords(float value) {
        return value * 1080f / Camera.main.pixelHeight;
    }

    public static Vector2 ScreenPointToHDCoords(Vector2 screenPoint) {
        return screenPoint * 1080f / Camera.main.pixelHeight;
    }

    public static string GetParsedHabilityDescription(Hability hability)
    {
        var description = hability.LocalizedDescription;
        description = description.Replace("{damage}", hability.Damage.ToString());
        return description;
    }

    public static HabilityCastEvent NewHabilityCastEvent(CreatureController target, float effectiveness)
    {
        return NewHabilityCastEvent(new CreatureController[]{target}, new float[]{effectiveness});
    }

    public static HabilityCastEvent NewHabilityCastEvent(CreatureController[] targets, float[] effectiveness)
    {
        //Debug.Log($"Creating an HabilityCastEvent. Selected hability is {GameState.selectedHability?.name}.");
        return new HabilityCastEvent{
            targets = targets,
            effectiveness = effectiveness,
            damage = GameState.selectedHability.Damage,
            damageType = GameState.selectedHability.DamageType,
        };
    }
}
