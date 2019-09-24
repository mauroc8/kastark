using UnityEngine;

public abstract class AlphaController : MonoBehaviour
{
    public virtual void ChangeAlpha(float alpha)
    {
        Debug.Log("Not Implemented.");
    }
    public virtual float GetAlpha()
    {
        Debug.Log("Not Implemented.");
        return -1;
    }
}
