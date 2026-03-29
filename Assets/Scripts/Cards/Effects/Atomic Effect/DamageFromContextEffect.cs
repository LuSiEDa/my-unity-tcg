using UnityEngine;
using System.Collections.Generic; // List를 쓰기 위해 필요
using System.Linq;               // [추가] Last()를 쓰기 위해 반드시 필요!

[CreateAssetMenu(fileName = "DamageFromContextEffect", menuName = "DamageFromContextEffect")]
public class DamageFromContextEffect : CardEffect
{
    // 파괴력을 만큼 데미지 효과
    public override void Execute(PlayerData user, PlayerData target, CardInstance instance, EffectContext context)
    {
        if (target == null) return;

        int damage = context.lastValue;

        var evt = new DamageCalculationEvent(user, damage, DamageSourceType.Effect, instance);

        EventBus.Publish(evt);

        target.ModifyLife(-evt.Damage);
    }
}
