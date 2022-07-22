using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public const float Width = 0.063f;
    public const float Height = 0.088f;

    private GameObject quadGameObject;
    private Material material;

    public Card()
    {
        var mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(Width / -2, Height / -2, 0),
            new Vector3(Width, Height / -2, 0),
            new Vector3(Width / -2, Height, 0),
            new Vector3(Width, Height, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        quadGameObject = new GameObject();
        material = new Material(Shader.Find("Custom/CardSurfaceShader"));

        var meshRenderer = quadGameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        var meshFilter = quadGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    public Vector3 Position
    {
        get
        {
            return quadGameObject.transform.position;
        }
        set
        {
            quadGameObject.transform.position = value;
        }
    }
}