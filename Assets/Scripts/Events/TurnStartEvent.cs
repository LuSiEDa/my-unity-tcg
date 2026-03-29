using UnityEngine;

public class TurnStartEvent
{
    public PlayerData Player;

    public TurnStartEvent(PlayerData player)
    {
        Player = player;
    }
}
