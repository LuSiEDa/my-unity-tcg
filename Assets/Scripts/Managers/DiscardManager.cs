using UnityEngine;
using System;
using System.Collections.Generic;
public class DiscardManager : MonoBehaviour
{
    public static DiscardManager Instance;

    public List<CardInstance> discardPile = new List<CardInstance>();
    public void Awake() => Instance = this;
    public void AddToDiscard(CardInstance instance)
    {
        if (instance == null) return;
        discardPile.Add(instance);
        EventBus.Publish(new CardMovedEvent(instance, CardZone.Discard));
    }
    public void ClearDiscard()
    {
        discardPile.Clear();
        EventBus.Publish(
            new DiscardPileChangedEvent(
                discardPile.Count,
                discardPile.Count > 0 ? discardPile[^1] : null
            )
        );
    }
}
