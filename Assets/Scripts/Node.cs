using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base node class, empty

public class Node : MonoBehaviour
{
    public Vector3 position = Vector3.negativeInfinity;
    public int scale = 1;

    public virtual void Start()
    {
        if (gameObject && this.position != Vector3.negativeInfinity)
            gameObject.transform.position = this.position;
    }

    public virtual void Init(int scale, Vector3 position)
    {
        this.scale = scale;
        this.position = position;
    }

    public virtual void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        //Gizmos.DrawSphere(position, (float)scale / 10f);
    }
}

// Node with a mesh, used for constructing game enviroments

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class NodeMesh : Node
{
    public Mesh nodeMesh;
    public MeshFilter meshFilter;

    public override void Start()
    {
        base.Start();

        CreateNodeMesh(scale);
        GetComponent<MeshFilter>().mesh = nodeMesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
    }

    void CreateNodeMesh(int size)
    {
        meshFilter = GetComponent<MeshFilter>();

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

        nodeMesh = mesh;
    }

    public override void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(position, (float)scale / 10f);
    }
}