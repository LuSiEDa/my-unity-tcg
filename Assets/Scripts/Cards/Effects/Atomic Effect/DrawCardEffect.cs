using UnityEngine;

[CreateAssetMenu(fileName = "DrawCardEffect", menuName = "CardEffects/DrawCardEffect")]
public class DrawCardEffect : CardEffect
{
    public int amount = 1;

    public override void Execute(PlayerData user, PlayerData target = null, CardInstance instance = null, EffectContext context = null)
    {
        if (user == null) return;

        for (int i = 0; i < amount; i++)
        {
            TurnManager.Instance.ExecutePlayerAction(user, ActionType.DrawCard);
        }
    }
}
