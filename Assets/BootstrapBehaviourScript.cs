using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BootstrapBehaviourScript : MonoBehaviour
{
    const int cardsCount = 5;
    const float cardsSpacing = 5;

    private List<Card> cards = new List<Card>(cardsCount);
    private Button loadButton;
    private Button stopButton;
    private Dropdown modeSelectionDropdown;

    void Awake()
    {
        for (int i = 0; i < cardsCount; i++)
        {
            var card = new Card();
            card.Position = new Vector3((i - cardsCount / 2.0f + 0.5f) * Card.Width * cardsSpacing, 0.0f, 0.0f);
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
    }

    void Update()
    {

    }

    public void OnLoadButtonClick()
    {
        Debug.Log("OnLoadButtonClick");
        var selectedMode = (LoadingMode)modeSelectionDropdown.value;
        Debug.Log(selectedMode);
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
