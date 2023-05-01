using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BSP_Generator))]
public class BSP_GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BSP_Generator generator = (BSP_Generator)target;

        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }
    }
}
