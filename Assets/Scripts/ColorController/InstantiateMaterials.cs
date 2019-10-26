using UnityEngine;

public class InstantiateMaterials : MonoBehaviour
{
    void Awake()
    {
        var renderer = GetComponent<Renderer>();

        for (int i = 0; i < renderer.materials.Length; i++)
        {
            renderer.materials[i] = Instantiate(renderer.materials[i]);
        }
    }
}
