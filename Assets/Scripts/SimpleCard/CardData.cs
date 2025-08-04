using UnityEngine;

[System.Serializable]
public class CardData
{
    public enum Suit { Spades, Hearts, Diamonds, Clubs }
    public enum Rank { Ace = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 11, Queen = 12, King = 13 }

    public Suit suit;
    public Rank rank;

    public CardData(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    // Returns the blackjack value of the card
    public int GetBlackjackValue()
    {
        if (rank == Rank.Ace)
            return 11; // In Blackjack, Ace can be 1 or 11
        else if (rank == Rank.Jack || rank == Rank.Queen || rank == Rank.King)
            return 10; // Face cards are worth 10
        else
            return (int)rank;
    }

    // Returns the display name of the card
    public string GetDisplayName()
    {
        string rankDisplay = "";

        switch (rank)
        {
            case Rank.Ace: rankDisplay = "A"; break;
            case Rank.Jack: rankDisplay = "J"; break;
            case Rank.Queen: rankDisplay = "Q"; break;
            case Rank.King: rankDisplay = "K"; break;
            default: rankDisplay = ((int)rank).ToString(); break;
        }

        return rankDisplay;
    }

    // Get color based on suit
    public Color GetSuitColor()
    {
        return (suit == Suit.Hearts || suit == Suit.Diamonds) ?
            new Color(0.9f, 0.1f, 0.1f) : // Red for Hearts/Diamonds
            Color.black; // Black for Spades/Clubs
    }

    // Add this method to your CardData class
    public int GetCardValue()
    {
        // For a standard card game like Blackjack:
        switch (rank)
        {
            case Rank.Ace:
                return 11; // In Blackjack, Ace can be 1 or 11
            case Rank.Two:
                return 2;
            case Rank.Three:
                return 3;
            case Rank.Four:
                return 4;
            case Rank.Five:
                return 5;
            case Rank.Six:
                return 6;
            case Rank.Seven:
                return 7;
            case Rank.Eight:
                return 8;
            case Rank.Nine:
                return 9;
            case Rank.Ten:
            case Rank.Jack:
            case Rank.Queen:
            case Rank.King:
                return 10;
            default:
                return 0;
        }
    }
}