using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapBehaviourScript : MonoBehaviour
{
    const int cardsCount = 5;
    const float cardsSpacingMultiplier = 5;

    private List<Card> cards = new List<Card>(cardsCount);
    private Button loadButton;
    private Button stopButton;
    private Dropdown modeSelectionDropdown;

    void Awake()
    {
        for (int i = 0; i < cardsCount; i++)
        {
            var card = new Card();
            card.Position = new Vector3((i - cardsCount / 2.0f + 0.5f) * Card.Width * cardsSpacingMultiplier, 0.0f, 0.0f);
            cards.Add(card);
        }

        loadButton = GameObject.Find("LoadButton").GetComponent<Button>();
        loadButton.onClick.AddListener(OnLoadButtonClick);

        stopButton = GameObject.Find("StopButton").GetComponent<Button>();
        stopButton.onClick.AddListener(OnStopButtonClick);

        modeSelectionDropdown = GameObject.Find("ModeSelectionDropdown").GetComponent<Dropdown>();
    }

    void Start()
    {
        StartCoroutine(OnAspectChangedCoroutine());
    }

    void Update()
    {

    }

    private float cameraAspect = 0.0f;

    private IEnumerator OnAspectChangedCoroutine()
    {
        while (true)
        {
            if (Camera.main.aspect != cameraAspect)
            {
                AdjustCameraToFitCards();
                cameraAspect = Camera.main.aspect;
            }
            yield return new WaitForSeconds(.2f);
        }
    }

    static void AdjustCameraToFitCards()
    {
        var frustumWidth = (cardsCount + 1) * Card.Width * cardsSpacingMultiplier;
        var frustumHeight = frustumWidth / Camera.main.aspect;
        var distance = frustumHeight * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, distance * -1);
        Camera.main.nearClipPlane = distance - Card.Width * 2;
    }

    public void OnLoadButtonClick()
    {
        Debug.Log("OnLoadButtonClick");
        var selectedMode = (LoadingMode)modeSelectionDropdown.value;
        Debug.Log(selectedMode);

        foreach (var card in cards)
        {
            StartCoroutine(PicsumApi.LoadCard(card));
        }
    }

    public void OnStopButtonClick()
    {
        Debug.Log("OnStopButtonClick");
    }
}

public enum LoadingMode
{
    AllAtOnce = 0,
    OneByOne = 1,
    WhenImageReady = 2,
}
