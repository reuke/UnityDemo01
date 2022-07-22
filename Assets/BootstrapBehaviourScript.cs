using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootstrapBehaviourScript : MonoBehaviour
{
    const int cardsCount = 5;
    const float cardsSpacing = 5;

    private List<Card> cards = new List<Card>(cardsCount);

    void Awake()
    {
        for (int i = 0; i < cardsCount; i++)
        {
            var card = new Card();
            card.Position = new Vector3((i - cardsCount / 2.0f + 0.5f) * Card.Width * cardsSpacing, 0.0f, 0.0f);
            cards.Add(card);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
