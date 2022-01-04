using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator gridGen = (GridGenerator)target;
        if (GUILayout.Button("Regenerate"))
        {
            gridGen.RegenerateGrid();
        }

        if (GUILayout.Button("Delete"))
        {
            gridGen.DeleteGrid();
        }
    }
}
