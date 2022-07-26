using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

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
        var mesh = Helpers.GetMesh(Width, Height);

        quadGameObject = new GameObject();
        material = new Material(Shader.Find("CardShader"));
        material.EnableKeyword("_MainTex");
        material.EnableKeyword("_BackTex");
        material.EnableKeyword("_FrontTex");
        material.EnableKeyword("_LightComponent");
        material.SetTexture("_BackTex", Helpers.BackTexture);

        var meshRenderer = quadGameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        var meshFilter = quadGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        quadGameObject.transform.eulerAngles = new Vector3(0, -180, 0);
        material.SetFloat("_LightComponent", FlipBackSideLightValue);
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
    public void SetTexture(Texture2D texture)
    {
        material.SetTexture("_MainTex", texture);
        material.SetTexture("_FrontTex", Helpers.GetClosestByColorFrontTexture(texture));
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
    const float FlipFrontSideLightValue = 0.0f;
    const float FlipBackSideLightValue = 0.1f;
    const float FlipRaiseScale = 0.66f;

    private void ShowFrontSide()
    {
        var sequence = DOTween.Sequence();
        sequence.Insert(0, quadGameObject.transform.DORotate(new Vector3(0, 360, 0), FlipTime, RotateMode.FastBeyond360));

        var vectorToCamera = Vector3.Normalize(Camera.main.transform.position - quadGameObject.transform.position);
        sequence.Insert(0, quadGameObject.transform.DOMove(quadGameObject.transform.position + vectorToCamera * Width * FlipRaiseScale, FlipTime / 2));
        sequence.Insert(FlipTime / 2, quadGameObject.transform.DOMove(quadGameObject.transform.position, FlipTime / 2));

        sequence.Insert(0, material.DOFloat(FlipFrontSideLightValue, "_LightComponent", FlipTime));

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

        sequence.Insert(0, material.DOFloat(FlipBackSideLightValue, "_LightComponent", FlipTime));

        sequence.onComplete += () =>
        {
            isFrontSideCurrent = false;
            StartFlipIfRequired();
        };
    }
}
