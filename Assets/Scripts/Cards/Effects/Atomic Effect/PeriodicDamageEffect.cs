using UnityEngine;

[CreateAssetMenu(fileName = "PeriodicDamageEffect", menuName = "CardEffects/PeriodicDamageEffect")]
public class PeriodicDamageEffect : CardEffect
{
    public int damageAmount = 10;

    public override void Execute(PlayerData user, PlayerData target = null, CardInstance instance = null, EffectContext context = null)
    {
        var evt = new DamageCalculationEvent(
            user,
            damageAmount,
            DamageSourceType.Effect,
            instance);
        EventBus.Publish(evt);
        user.ModifyLife(-evt.Damage);
    }
}