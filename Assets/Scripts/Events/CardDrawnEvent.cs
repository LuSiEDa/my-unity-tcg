using UnityEngine;

public readonly struct CardDrawnEvent
{
    public readonly PlayerData player;
    public readonly CardInstance instance;
    public readonly DrawReason reason;

    public CardDrawnEvent(PlayerData player, CardInstance instance, DrawReason reason)
    {
        this.player = player;
        this.instance = instance;
        this.reason = reason;
    }
}
