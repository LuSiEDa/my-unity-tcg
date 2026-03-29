using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> cards;
    public Deck(List<Card> initCards)
    {
        cards = new List<Card>(initCards);
    }
    public bool IsEmpty() => cards.Count == 0;

    public Card Draw()
    {
        if (IsEmpty()) return null;
        Card top = cards[0];
        cards.RemoveAt(0);
        return top;
    }
}
