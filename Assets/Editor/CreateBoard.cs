using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Map))]
public class CreateBoard : Editor
{
    Map gen;
    int i = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create interior board"))
        {
            if (gen.grid.Length != 0)
            {
                gen.DestroyMap();
                gen.CreateMapInterior();
            }
            else
            {
                if (i == 0)
                {
                    gen.CreateMapInterior();
                    i = 1;
                }
            }
        }

        if (GUILayout.Button("Create exterior board"))
        {
            if (gen.grid.Length != 0)
            {
                gen.DestroyMap();
                gen.CreateMapExterior();
            }
            else
            {
                if (i == 0)
                {
                    gen.CreateMapExterior();
                    i = 1;
                }
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginVertical();

        if (Application.isPlaying)
        {
            if (GUILayout.Button("Create Nodes"))
            {
                if (gen.grid.Length != 0)
                {
                    if (gen.mapType == MapType.INTERIOR_EMPTY)
                    {
                        gen.CreateNodesInterior();
                    }
                    else if (gen.mapType == MapType.EXTERIOR_EMPTY)
                    {
                        gen.CreateNodesExterior();
                    }
                }
            }
        }

        if (GUILayout.Button("Destroy board"))
        {
            if (gen.grid.Length != 0)
            {
                if (i == 1)
                    i = 0;

                gen.DestroyMap();
            }
        }

        GUILayout.EndVertical();
    }
    
    private void OnEnable()
    {
        gen = target as Map;
    }
}
