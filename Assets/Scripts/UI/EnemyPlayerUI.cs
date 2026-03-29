using UnityEngine;
using TMPro;

public class EnemyPlayerUI : PlayerUI
{
    [Header("Enemy Only")]
    [SerializeField]
    private TextMeshProUGUI handCount;
    
    public override void Setup(PlayerData playerData)
    {
        base.Setup(playerData);
        Refresh();
    }
    public override void Refresh()
    {
        base.Refresh();
        if (Data == null) return;

        if (handCount != null)
            handCount.text = $"Hand : {Data.hand.Count}";
    }
    public override Transform GetZone(CardZone zone)
    {
        if (zone == CardZone.Hand)
            return null; // Enemy는 핸드 UI 없음

        return base.GetZone(zone);
    }
}
