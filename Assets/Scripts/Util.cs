using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static GameObject[] GetGameObjectChildren(GameObject root) {
        GameObject[] children = new GameObject[root.transform.childCount];

        int i = 0;
        foreach (Transform child in root.transform) {
            children[i++] = child.gameObject;
        }

        return children;
    }
}
