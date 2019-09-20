using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base node class, empty
// Probably shouldn't use this for spawning, but to hold all the functions for all the nodes

public class Node : MonoBehaviour
{
    public Vector3 position = Vector3.negativeInfinity;
    public int scale = 1;

    public virtual void Start()
    {
        
    }

    public virtual void Init(int scale, Vector3 position)
    {
        this.scale = scale;
        this.position = position;

        if (gameObject && position != Vector3.negativeInfinity)
            gameObject.transform.position = this.position;
    }
}

// Node with a mesh, used for constructing game enviroments

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class NodeMesh : Node
{
    public Mesh nodeMesh;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    
    public override void Init(int scale, Vector3 position)
    {
        base.Init(scale, position);

        CreateNodeMesh(scale);
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = nodeMesh;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    //TODO: Add particle effect when node mesh lands at destination. Some shake would be good too.
    public IEnumerator Move(Vector3 startPos, Vector3 endPos, float timeValue)
    {
        float r = 1.0f / timeValue;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * r;
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
    }

    public void SetColor(Material material)
    {
        if (!meshRenderer)
            meshRenderer = GetComponent<MeshRenderer>();

        meshRenderer.material = material;
    }

    private void CreateNodeMesh(int size)
    {
        Mesh mesh = new Mesh();

        Vector3 v0 = new Vector3(-scale * .5f, -scale * .5f, scale * .5f);
        Vector3 v1 = new Vector3(scale * .5f, -scale * .5f, scale * .5f);
        Vector3 v2 = new Vector3(scale * .5f, -scale * .5f, -scale * .5f);
        Vector3 v3 = new Vector3(-scale * .5f, -scale * .5f, -scale * .5f);

        Vector3 v4 = new Vector3(-scale * .5f, scale * .5f, scale * .5f);
        Vector3 v5 = new Vector3(scale * .5f, scale * .5f, scale * .5f);
        Vector3 v6 = new Vector3(scale * .5f, scale * .5f, -scale * .5f);
        Vector3 v7 = new Vector3(-scale * .5f, scale * .5f, -scale * .5f);

        mesh.vertices = new Vector3[]
        {
            v0, v1, v2, v3,
            v7, v4, v0, v3,
            v4, v5, v1, v0,
            v6, v7, v3, v2,
            v5, v6, v2, v1,
            v7, v6, v5, v4
        };

        int faces = 6;

        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < faces; i++)
        {
            int triangleOffset = i * 4;
            triangles.Add(0 + triangleOffset);
            triangles.Add(2 + triangleOffset);
            triangles.Add(1 + triangleOffset);

            triangles.Add(0 + triangleOffset);
            triangles.Add(3 + triangleOffset);
            triangles.Add(2 + triangleOffset);

            // same uvs for all faces
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));
        }

        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        if (!meshRenderer)
            meshRenderer.material = new Material(Shader.Find("Standard"));

        nodeMesh = mesh;
    }
}