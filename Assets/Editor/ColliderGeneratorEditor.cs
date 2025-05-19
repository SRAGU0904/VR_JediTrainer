using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ColliderGenerator))]
public class ColliderGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ColliderGenerator generator = (ColliderGenerator)target;

        if (GUILayout.Button("Generate Box Collider"))
        {
            generator.GenerateMeshCollider();
        }
    }
}
