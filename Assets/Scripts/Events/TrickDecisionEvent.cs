using UnityEngine;

public class TrickDecisionEvent
{
    public PlayerData player;
    public CardInstance instance;
    public float timeout;
    // UI 요청용

    public TrickDecisionEvent(PlayerData player, CardInstance instance, float timeout)
    {
        this.player = player;
        this.instance = instance;
        this.timeout = timeout;
    }
}
