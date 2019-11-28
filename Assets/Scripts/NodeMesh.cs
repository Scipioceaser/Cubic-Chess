using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Node with a mesh, used for constructing game enviroments

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class NodeMesh : Node
{
    public Mesh nodeMesh;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Material color;

    public override void Init(int scale, Vector3 position)
    {
        base.Init(scale, position);

        CreateNodeMesh();
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

            //if (t >= 1.0f)
            //    Globals.meshNodesCreated++;

            yield return null;
        }
    }

    public void SetColor(Color color)
    {
        if (!meshFilter)
            meshFilter = GetComponent<MeshFilter>();

        Mesh mesh = meshFilter.sharedMesh;

        if (mesh != null)
        {
            Vector3[] vertices = mesh.vertices;

            Color[] colors = new Color[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
                colors[i] = color;

            mesh.colors = colors;
        }
    }

    //TODO: Deal with mesh color blending
    public void SetFaceColor(Node node, Color color)
    {
        Vector3 faceCenterPoint = GetMiddlePoint(node.position, position);
        CreatePlaneMesh(faceCenterPoint, color);
    }

    private Vector3 GetMiddlePoint(Vector3 A, Vector3 B)
    {
        return (A + (0.5f * (B - A).normalized));
    }

    public void SetRender(bool toRender)
    {
        meshRenderer.enabled = toRender;
    }

    private void CreatePlaneMesh(Vector3 position, Color color)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale *= (scale / 15f);
        Vector3 d = (position - transform.position);
        plane.transform.position = position + (d * 0.1f);
        plane.transform.rotation = Quaternion.FromToRotation(Vector3.up, d);
        plane.tag = "ColorPlane";
        plane.transform.parent = transform;
        plane.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        plane.GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Node"));
        plane.GetComponent<MeshRenderer>().sharedMaterial.color = color;
        Destroy(plane.GetComponent<MeshCollider>());
    }

    private void CreateNodeMesh()
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
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Node"));
        }

        nodeMesh = mesh;
    }
}