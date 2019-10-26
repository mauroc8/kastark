using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RendererMultiColorController : MultiColorController
{
    List<Material> _materials;

    void Awake()
    {
        _materials = new List<Material>(
            GetComponent<Renderer>().materials
        );
    }

    public override List<Color> MyColors
    {
        get { return _materials.ConvertAll(material => material.color); }
        set {
            Debug.Assert(_materials.Count == value.Count);

            for (int i = 0; i < value.Count; i++)
                _materials[i].color = value[i];
        }
    }
}
