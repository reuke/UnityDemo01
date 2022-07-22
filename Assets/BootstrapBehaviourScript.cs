using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapBehaviourScript : MonoBehaviour
{
    const int cardsCount = 5;
    const float cardsSpacingMultiplier = 1.2f;

    private List<Card> cards = new List<Card>(cardsCount);
    private Button loadButton;
    private Button stopButton;
    private Dropdown modeSelectionDropdown;

    void Awake()
    {
        DOTween.Init();
        DOTween.defaultEaseType = Ease.InOutQuad;

        TextureHelpers.InitFrontTextures();

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
        if ((LoadingMode)modeSelectionDropdown.value == LoadingMode.AllAtOnce)
        {
            runningCoroutines.Add(StartCoroutine(LoadAllAtOnce()));
        }
        else if ((LoadingMode)modeSelectionDropdown.value == LoadingMode.OneByOne)
        {
            runningCoroutines.Add(StartCoroutine(LoadOneByOne()));
        }
        else if ((LoadingMode)modeSelectionDropdown.value == LoadingMode.WhenImageReady)
        {
            runningCoroutines.Add(StartCoroutine(LoadWhenImageReady()));
        }

        UpdateButtons();
    }

    private List<Coroutine> runningCoroutines = new List<Coroutine>();

    public void OnStopButtonClick()
    {
        foreach (var runningCoroutine in runningCoroutines)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutines.Clear();
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        loadButton.enabled = !runningCoroutines.Any();
        stopButton.enabled = !loadButton.enabled;
        modeSelectionDropdown.enabled = loadButton.enabled;
    }

    private IEnumerator LoadAllAtOnce()
    {
        cards.ForEach(t => t.IsFrontSide = false);

        var cardsCoroutines = cards
            .Select(t => StartCoroutine(PicsumApi.LoadCard(t)))
            .ToArray();

        runningCoroutines.AddRange(cardsCoroutines);

        foreach (var coroutine in cardsCoroutines)
            yield return coroutine;

        cards.ForEach(t => t.IsFrontSide = true);

        runningCoroutines.Clear();
        UpdateButtons();
    }

    private IEnumerator LoadOneByOne()
    {
        cards.ForEach(t => t.IsFrontSide = false);

        foreach (var card in cards)
        {
            var coroutine = StartCoroutine(PicsumApi.LoadCard(card));
            runningCoroutines.Add(coroutine);
            yield return coroutine;
            card.IsFrontSide = true;
        }

        runningCoroutines.Clear();
        UpdateButtons();
    }

    private IEnumerator LoadWhenImageReady()
    {
        cards.ForEach(t => t.IsFrontSide = false);

        var cardsCoroutines = cards.Select(t => StartCoroutine(LoadWhenImageReady(t))).ToArray();

        foreach (var coroutine in cardsCoroutines)
            runningCoroutines.Add(coroutine);

        foreach (var coroutine in cardsCoroutines)
            yield return coroutine;

        runningCoroutines.Clear();
        UpdateButtons();
    }

    private IEnumerator LoadWhenImageReady(Card card)
    {
        var coroutine = StartCoroutine(PicsumApi.LoadCard(card));
        runningCoroutines.Add(coroutine);
        yield return coroutine;
        card.IsFrontSide = true;
    }
}

public enum LoadingMode
{
    AllAtOnce = 0,
    OneByOne = 1,
    WhenImageReady = 2,
}
