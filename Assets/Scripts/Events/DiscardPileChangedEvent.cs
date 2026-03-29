using UnityEngine;

public readonly struct DiscardPileChangedEvent
{
    public readonly int count;
    public readonly CardInstance topCard;
    public DiscardPileChangedEvent(int count, CardInstance topCard)
    {
        this.count = count;
        this.topCard =topCard;
    }
}
