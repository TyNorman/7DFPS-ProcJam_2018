using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof (TerrainGenerator))]
public class EditorMapPreview : Editor
{
    public override void OnInspectorGUI()
    {
        TerrainGenerator terrainGenerator = (TerrainGenerator)target;

        if (DrawDefaultInspector() )
        {
            if (terrainGenerator.AutoUpdate)
                terrainGenerator.GenerateMap();
        }

        if (GUILayout.Button("Generate"))
            terrainGenerator.GenerateMap();

        //base.OnInspectorGUI();
    }
}
#endif
