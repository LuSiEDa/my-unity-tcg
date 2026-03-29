using UnityEngine;

[CreateAssetMenu(fileName = "NullifyDamageEffect", menuName = "CardEffects/NullifyDamageEffect")]
public class NullifyDamageEffect : CardEffect
{
    public override void Execute(PlayerData user, PlayerData target = null, CardInstance instance = null, EffectContext context = null)
    {
        // 직접 실행 로직 없음
    }
    public override void ModifyDamage(ref int damage)
    {
        damage = 0;
    }

}
