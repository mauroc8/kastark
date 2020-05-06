using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

[System.Serializable]
public struct CameraVariables
{
    public float distance;
    public float fieldOfView;
    public float rotation;
    public float elevation;

    public static
    CameraVariables FromWorld(
        Transform focusTransform,
        Transform cameraTransform,
        Camera cameraComponent)
    {
        return new CameraVariables
        {
            distance =
                cameraTransform.localPosition.z,
            fieldOfView =
                cameraComponent.fieldOfView,
            rotation =
                focusTransform.eulerAngles.y,
            elevation =
                focusTransform.eulerAngles.x
        };
    }

    public void ApplyToWorld(
        Transform focusTransform,
        Transform cameraTransform,
        Camera cameraComponent)
    {
        cameraTransform.localPosition =
            new Vector3(0.0f, 0.0f, distance);

        cameraComponent.fieldOfView =
            fieldOfView;

        focusTransform.rotation =
            Quaternion.Euler(elevation, rotation, 0.0f);
    }

    public static
    CameraVariables Lerp(CameraVariables a, CameraVariables b, float t)
    {
        if (t < 0.0f)
            t = 0.0f;

        if (t > 1.0f)
            t = 1.0f;

        return new CameraVariables
        {
            distance =
                Mathf.Lerp(a.distance, b.distance, t),
            fieldOfView =
                Mathf.Lerp(a.fieldOfView, b.fieldOfView, t),
            rotation =
                Angle.Lerp(a.rotation * Angle.Degrees, b.rotation * Angle.Degrees, t) / Angle.Degrees,
            elevation =
                Angle.Lerp(a.elevation * Angle.Degrees, b.elevation * Angle.Degrees, t) / Angle.Degrees
        };
    }
}

public class CameraController : MonoBehaviour
{
    public StateStream<CameraVariables> variables =
        new StateStream<CameraVariables>(new CameraVariables { });

    void Awake()
    {
        var cameraTransform =
            Query.From(this, "camera").Get<Transform>();

        var cameraComponent =
            Query.From(this, "camera").Get<Camera>();

        variables.Value =
            CameraVariables.FromWorld(transform, cameraTransform, cameraComponent);

        variables.Get(value =>
        {
            value.ApplyToWorld(transform, cameraTransform, cameraComponent);
        });
    }

    public void ApplyVariables()
    {
        var cameraTransform =
            Query.From(this, "camera").Get<Transform>();

        var cameraComponent =
            Query.From(this, "camera").Get<Camera>();

        variables.Value.ApplyToWorld(transform, cameraTransform, cameraComponent);
    }

    public void Init()
    {
        var cameraTransform =
            Query.From(this, "camera").Get<Transform>();

        var cameraComponent =
            Query.From(this, "camera").Get<Camera>();

        variables.Value =
            CameraVariables.FromWorld(transform, cameraTransform, cameraComponent);
    }
}
