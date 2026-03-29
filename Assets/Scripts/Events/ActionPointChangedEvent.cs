using UnityEngine;

public struct ActionPointChangedEvent
{
    public PlayerData player;
    public int currentAP;

    public ActionPointChangedEvent(PlayerData player, int ap)
    {
        this.player = player;
        currentAP = ap;
    }
}
