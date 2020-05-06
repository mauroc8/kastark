using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trebol : UpdateAsStream
{
    void Awake()
    {
        var transform =
            this.transform;

        update
            .Get(_ =>
            {
                transform.rotation =
                    Quaternion.Euler(
                        0.0f,
                        Time.time * 15.0f,
                        0.0f);
            });
    }
}
