using UnityEngine;

public class TrickSelectedEvent
{
    public PlayerData player;
    public CardInstance instance; // null이면 안함
    
    // 선택 결과
    public TrickSelectedEvent(PlayerData player, CardInstance instance)
    {
        this.player = player;
        this.instance = instance;
    }
}
