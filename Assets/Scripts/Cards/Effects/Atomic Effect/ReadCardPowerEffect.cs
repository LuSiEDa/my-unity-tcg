using UnityEngine;
using System.Collections.Generic; // List를 쓰기 위해 필요
using System.Linq;               // [추가] Last()를 쓰기 위해 반드시 필요!

[CreateAssetMenu(fileName = "ReadCardPowerEffect", menuName = "CardEffects/ReadCardPowerEffect")]
public class ReadCardPowerEffect : CardEffect
{
    // 카드 파괴력 참조 효과
    public override void Execute(PlayerData user, PlayerData target, CardInstance instance, EffectContext context)
    {
        if (context.lastValue < 0) return;

        context.lastValue = context.lastCard.origin.basePower;
    }
}
