using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraToolComponent))]
public class CameraToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cameraToolComponent =
            ((CameraToolComponent)target);

        var cameraController =
            cameraToolComponent
                .GetComponent<CameraController>();

        cameraController.Init();

        var cameraVariables =
            cameraController.variables.Value;

        cameraVariables.distance =
            EditorGUILayout.Slider("Distance", cameraVariables.distance, -300.0f, 0.0f);

        cameraVariables.fieldOfView =
            EditorGUILayout.Slider("FieldOfView", cameraVariables.fieldOfView, 5.0f, 80.0f);

        cameraVariables.rotation =
            EditorGUILayout.Slider("Rotation", cameraVariables.rotation, 0.0f, 360.0f);

        cameraVariables.elevation =
            EditorGUILayout.Slider("Elevation", cameraVariables.elevation, 0.0f, 90.0f);

        cameraController.variables.Value =
            cameraVariables;

        cameraController.ApplyVariables();

    }
}
