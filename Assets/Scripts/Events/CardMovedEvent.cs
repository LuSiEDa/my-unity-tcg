using UnityEngine;

public readonly struct CardMovedEvent
{
    public readonly CardInstance instance;
    public readonly CardZone targetZone;

    public CardMovedEvent(CardInstance instance, CardZone zone)
    {
        this.instance = instance;
        targetZone = zone;
    }
}
