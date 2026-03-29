using UnityEngine;

public readonly struct CurrentPlayerChangedEvent
{
    public readonly PlayerData currentPlayer;
    public CurrentPlayerChangedEvent(PlayerData player)
    {
        currentPlayer = player;
    }
}
