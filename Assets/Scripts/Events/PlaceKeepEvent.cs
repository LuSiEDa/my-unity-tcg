using UnityEngine;

public class PlaceKeepEvent
{
    public PlayerData user;
    public PlayerData target;
    public CardInstance instance;

    public PlaceKeepEvent(PlayerData user, PlayerData target, CardInstance instance)
    {
        this.user = user;
        this.target = target;
        this.instance = instance;
    }
}
