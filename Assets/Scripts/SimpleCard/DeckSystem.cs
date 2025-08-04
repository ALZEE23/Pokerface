using System.Collections.Generic;
using UnityEngine;

public class DeckSystem : MonoBehaviour
{
    // Singleton instance
    public static DeckSystem instance;

    [SerializeField] private List<CardData> cards = new List<CardData>();
    public Transform deckTransform;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeDeck();
        ShuffleDeck();

    }

    // Inisialisasi deck standar 52 kartu
    public void InitializeDeck()
    {
        cards.Clear();

        foreach (CardData.Suit suit in System.Enum.GetValues(typeof(CardData.Suit)))
        {
            foreach (CardData.Rank rank in System.Enum.GetValues(typeof(CardData.Rank)))
            {
                cards.Add(new CardData(suit, rank));
            }
        }
    }

    // Shuffle deck
    public void ShuffleDeck()
    {
        System.Random rng = new System.Random();
        int n = cards.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            CardData temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
    }

    // Deal satu kartu ke holder (fungsi paling sederhana)
    public Card DealCard(CardHolder holder)
    {
        if (cards.Count > 0)
        {
            CardData cardData = cards[0];
            cards.RemoveAt(0);
            Card card = holder.AddCard(cardData);
            return card;
        }
        else
        {
            Debug.Log("Deck kosong!");
            return null;
        }
    }

    // Deal beberapa kartu sekaligus
    public void DealCards(CardHolder holder, int count)
    {
        for (int i = 0; i < count && cards.Count > 0; i++)
        {
            DealCard(holder);
        }
    }

    // Mendapatkan jumlah kartu yang tersisa
    public int GetRemainingCards()
    {
        return cards.Count;
    }
}