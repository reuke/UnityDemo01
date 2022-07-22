using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card
{
    public const float Width = 63f;
    public const float Height = 88f;

    private GameObject quadGameObject;
    private Material material;

    private bool isFrontSideCurrent = false;
    private bool isFrontSideRequired = false;
    private bool isFlippingRightNow = false;
    public Card()
    {
        var mesh = new Mesh();

        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(Width / -2, Height / -2, 0),
            new Vector3(Width / 2, Height / -2, 0),
            new Vector3(Width / -2, Height / 2, 0),
            new Vector3(Width / 2, Height / 2, 0)
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
        material = new Material(Shader.Find("CardShader"));
        material.EnableKeyword("_MainTex");

        var meshRenderer = quadGameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        var meshFilter = quadGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        quadGameObject.transform.eulerAngles = new Vector3(0, -180, 0);
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
    public void SetTexture(Texture texture)
    {
        material.SetTexture("_MainTex", texture);
    }

    public bool IsFrontSide
    {
        get => isFrontSideRequired;
        set
        {
            isFrontSideRequired = value;
            if (!isFlippingRightNow)
            {
                StartFlipIfRequired();
            }
        }
    }

    private void StartFlipIfRequired()
    {
        isFlippingRightNow = false;
        if (isFrontSideCurrent != isFrontSideRequired)
        {
            isFlippingRightNow = true;
            if(isFrontSideRequired)
            {
                ShowFrontSide();
            }
            else
            {
                ShowBackSide();
            }
        }
    }

    const float FlipTime = 0.3f;
    const float FlipRaiseScale = 0.66f;

    private void ShowFrontSide()
    {
        var sequence = DOTween.Sequence();
        sequence.Insert(0, quadGameObject.transform.DORotate(new Vector3(0, 360, 0), FlipTime, RotateMode.FastBeyond360));

        var vectorToCamera = Vector3.Normalize(Camera.main.transform.position - quadGameObject.transform.position);
        sequence.Insert(0, quadGameObject.transform.DOMove(quadGameObject.transform.position + vectorToCamera * Width * FlipRaiseScale, FlipTime / 2));
        sequence.Insert(FlipTime / 2, quadGameObject.transform.DOMove(quadGameObject.transform.position, FlipTime / 2));

        sequence.onComplete += () =>
        {
            isFrontSideCurrent = true;
            StartFlipIfRequired();
        };
    }
    private void ShowBackSide()
    {
        var sequence = DOTween.Sequence();
        sequence.Insert(0, quadGameObject.transform.DORotate(new Vector3(0, 180, 0), FlipTime));

        var vectorToCamera = Vector3.Normalize(Camera.main.transform.position - quadGameObject.transform.position);
        sequence.Insert(0, quadGameObject.transform.DOMove(quadGameObject.transform.position + vectorToCamera * Width * FlipRaiseScale, FlipTime / 2));
        sequence.Insert(FlipTime / 2, quadGameObject.transform.DOMove(quadGameObject.transform.position, FlipTime / 2));

        sequence.onComplete += () =>
        {
            isFrontSideCurrent = false;
            StartFlipIfRequired();
        };
    }
}
