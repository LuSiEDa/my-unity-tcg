using System;

public class TrickResponse
{
    public PlayerData player;
    public CardInstance card;

    public TrickResponse(PlayerData p, CardInstance c)
    {
        player = p;
        card = c;
    }
}