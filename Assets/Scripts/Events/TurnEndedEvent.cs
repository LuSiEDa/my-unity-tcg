using UnityEngine;

public struct TurnEndedEvent
{
    public PlayerData CurrentPlayer;

    public TurnEndedEvent(PlayerData player)
    {
        CurrentPlayer = player;
    }
}
