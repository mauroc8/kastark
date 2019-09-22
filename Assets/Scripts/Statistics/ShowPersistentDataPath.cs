using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowPersistentDataPath : MonoBehaviour
{
    void Start()
    {
        GetComponent<Text>().text = Application.persistentDataPath;
    }
}
