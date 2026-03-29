using UnityEngine;

public class DamageCalculationEvent
{
    public PlayerData Target;
    public int Damage;

    public DamageSourceType SourceType;

    public CardInstance SourceCard;

    public DamageCalculationEvent(PlayerData target, int damage, DamageSourceType sourceType, CardInstance sourceCard = null)
    {
        Target = target;
        Damage = damage;
        SourceType = sourceType;
        SourceCard = sourceCard;
    }
}