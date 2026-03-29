using UnityEngine;
using System.Collections.Generic; // List를 쓰기 위해 필요
using System.Linq;               // [추가] Last()를 쓰기 위해 반드시 필요!

[CreateAssetMenu(fileName = "KeepConsumeEffect", menuName = "CardEffects/KeepConsumeEffect")]
public class KeepConsumeEffect : CardEffect
{
    // 지속 카드 파괴 효과
    public override void Execute(PlayerData user, PlayerData target, CardInstance instance, EffectContext context)
    {
        if (target == null || target.activeKeepCard == null)
        {
            Debug.LogWarning("대상 플레이어나 카드가 지정되지 않음");
            return;
        }
        var keep = target.activeKeepCard;
        if (keep == null) return;

        context.lastCard = keep;
        target.SetKeepCard(null);

        GameActionController.Instance.MoveCardPublic(keep, CardZone.Discard);
    }
}
