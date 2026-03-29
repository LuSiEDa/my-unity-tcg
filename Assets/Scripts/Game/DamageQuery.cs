using UnityEngine;

public class DamageQuery
{
    public int damage;
    public PlayerData target;
    public DamageQuery(int damage, PlayerData target)
    {
        this.damage = damage;
        this.target = target;
    }
}
