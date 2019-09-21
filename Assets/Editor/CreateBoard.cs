using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Generate))]
public class CreateBoard : Editor
{
    Generate gen;
    int i = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Board"))
        {
            if (gen.grid.Length != 0)
            {
                gen.DestroyMap();
                gen.CreateMap();
            }
            else
            {
                if (i == 0)
                {
                    gen.CreateMap();
                    i = 1;
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

        GUILayout.EndHorizontal();
    }
    
    private void OnEnable()
    {
        gen = target as Generate;
    }
}
