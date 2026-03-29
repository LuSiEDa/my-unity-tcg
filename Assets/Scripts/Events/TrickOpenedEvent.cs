using UnityEngine;

public class TrickOpenedEvent
{
    public PlayerData user;
    public CardInstance instance;

    public TrickOpenedEvent(PlayerData user, CardInstance instance)
    {
        this.user = user;
        this.instance = instance;
    }
}
