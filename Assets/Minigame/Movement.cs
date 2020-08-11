using UnityEngine;

public class Movement
{
    public Block block;
    public (int, int) target;
    public float startTime;

    public static float Duration =>
        0.3f;
}
