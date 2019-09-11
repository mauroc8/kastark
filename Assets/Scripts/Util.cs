using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static string HabilityNameFromId(HabilityId id) {
        switch (id) {
            case HabilityId.Attack:
            return "attack";
            case HabilityId.Magic:
            return "magic";
            case HabilityId.Shield:
            return "shield";
            case HabilityId.Heal:
            return "heal";
        }
        Debug.LogError($"HabilityNameFromId({id}) is unknown");
        return id.ToString();
    }
}
