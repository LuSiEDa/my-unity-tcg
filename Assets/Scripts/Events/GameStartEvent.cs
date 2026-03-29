using UnityEngine;
using System.Collections.Generic;

public readonly struct GameStartEvent
{
    public readonly List<PlayerData> players;
    public readonly PlayerData localPlayer;

    public GameStartEvent(List<PlayerData> players, PlayerData localPlayer)
    {
        this.players = players;
        this.localPlayer = localPlayer;
    }
}
