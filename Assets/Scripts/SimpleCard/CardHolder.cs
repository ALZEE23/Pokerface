using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float cardSpacing = 0.5f;
    [SerializeField] private bool spreadCards = true;
    [SerializeField] private bool openFirstCard = true;

    private List<Card> cards = new List<Card>();

    // Method untuk menambahkan kartu ke holder
    public Card AddCard(CardData cardData)
    {
        // Debug
        Debug.Log("Adding card to holder: " + gameObject.name);

        // Buat instance kartu baru
        GameObject cardObject = Instantiate(cardPrefab, transform);
        cardObject.name = "Card_" + cardData.suit + "_" + cardData.rank;

        Card card = cardObject.GetComponent<Card>();
        if (card == null)
        {
            Debug.LogError("Prefab kartu tidak memiliki komponen Card!");
            return null;
        }

        // Set data kartu dulu
        card.data = cardData;

        // Lalu buat visual-nya
        card.CreateVisual();

        // Tambahkan ke list
        cards.Add(card);

        // Atur ulang posisi semua kartu
        ArrangeCards();

        // Jika kartu pertama dan openFirstCard aktif, buka kartu
        if (openFirstCard && cards.Count == 1)
        {
            if (card.cardVisual != null)
            {
                // Beri delay agar animasi kartu datang selesai dulu
                StartCoroutine(OpenCardAfterDelay(card.cardVisual));
            }
        }

        Debug.Log("Card added and visual created. Check VisualCardsHandler now.");
        return card;
    }

    private IEnumerator OpenCardAfterDelay(CardVisual cardVisual)
    {
        // Tunggu 5 detik (atau sesuaikan dengan initialDelayAtDeck + moveDuration)
        yield return new WaitForSeconds(5.5f);

        // Buka kartu
        cardVisual.OpenCard();
    }

    // Method untuk menghapus kartu
    public void RemoveCard(Card card)
    {
        if (cards.Contains(card))
        {
            cards.Remove(card);
            Destroy(card.gameObject);
            ArrangeCards();
        }
    }

    // Method untuk mengosongkan holder
    public void ClearCards()
    {
        foreach (Card card in cards)
        {
            Destroy(card.gameObject);
        }
        cards.Clear();
    }

    // Method untuk mengatur posisi kartu
    private void ArrangeCards()
    {
        if (cards.Count == 0) return;

        float startX;

        if (spreadCards)
        {
            // Spread cards evenly
            startX = -(cardSpacing * (cards.Count - 1)) / 2f;

            for (int i = 0; i < cards.Count; i++)
            {
                Vector3 pos = new Vector3(startX + (i * cardSpacing), 0, 0);
                cards[i].transform.localPosition = pos;
            }
        }
        else
        {
            // Stack cards with slight offset
            for (int i = 0; i < cards.Count; i++)
            {
                Vector3 pos = new Vector3(i * 0.05f, i * 0.02f, -i * 0.01f);
                cards[i].transform.localPosition = pos;
            }
        }
    }

    // Method untuk mendapatkan semua kartu
    public List<Card> GetCards()
    {
        return cards;
    }

    // Method untuk mendapatkan jumlah kartu
    public int GetCardCount()
    {
        return cards.Count;
    }

    public int CalculateScore()
    {
        int score = 0;
        int aceCount = 0;

        // Pertama, hitung semua kartu non-Ace dan hitung jumlah Ace
        foreach (Card card in cards)
        {
            if (card.data != null)
            {
                // Jika kartu adalah Ace, tambahkan ke counter
                if (card.data.rank == CardData.Rank.Ace)
                {
                    aceCount++;
                }
                else
                {
                    // Tambahkan nilai kartu non-Ace
                    score += card.data.GetCardValue();
                }
            }
        }

        // Sekarang hitung nilai Ace secara optimal
        for (int i = 0; i < aceCount; i++)
        {
            // Jika menambahkan 11 masih di bawah atau sama dengan 21, gunakan 11
            if (score + 11 <= 21)
            {
                score += 11;
            }
            // Jika tidak, gunakan 1
            else
            {
                score += 1;
            }
        }

        return score;
    }
}