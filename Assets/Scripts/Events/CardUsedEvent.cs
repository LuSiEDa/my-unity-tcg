using UnityEngine;

public class CardUsedEvent
{
    public PlayerData user;
    public CardInstance instance;

    public CardUsedEvent(PlayerData user, CardInstance instance)
    {
        this.user = user;
        this.instance = instance;
    }
}
