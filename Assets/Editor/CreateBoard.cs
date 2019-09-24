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

        if (gen.mapType != MapType.NONE)
        {
            if (GUILayout.Button("Create board"))
            {
                if (gen.grid.Length != 0 || i == 0)
                {
                    if (i == 0)
                        i = 1;

                    if (gen.mapType == MapType.EXTERIOR_EMPTY)
                    {
                        gen.DestroyMap();
                        gen.CreateMapExterior();
                    }
                    else if (gen.mapType == MapType.INTERIOR_EMPTY)
                    {
                        gen.DestroyMap();
                        gen.CreateMapInterior();
                    }
                }
            }
        }

        if (gen.mapType == MapType.NONE && gen.grid.Length != 0)
        {
            if (i == 1)
                i = 0;

            gen.DestroyMap();
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

        //if (GUILayout.Button("Destroy board"))
        //{
        //    if (gen.grid.Length != 0)
        //    {
        //        if (i == 1)
        //            i = 0;
        //
        //        gen.DestroyMap();
        //    }
        //}

        GUILayout.EndVertical();
    }
    
    private void OnEnable()
    {
        gen = target as Map;
    }
}
