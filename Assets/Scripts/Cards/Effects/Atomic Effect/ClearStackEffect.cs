using UnityEngine;

[CreateAssetMenu(fileName = "ClearStackEffect", menuName = "CardEffects/ClearStackEffect")]
public class ClearStackEffect : CardEffect
{
    public override void Execute(PlayerData user, PlayerData target = null, CardInstance instance = null, EffectContext context = null)
    {
        StackManager.Instance.ForceClearAllStacks();
    }
}
