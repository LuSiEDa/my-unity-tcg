using UnityEngine;

[CreateAssetMenu(fileName = "EmpressCaptureEffect", menuName = "CardEffects/EmpressCapture")]
public class EmpressCaptureEffect : CardEffect
{
    public override void Execute(PlayerData user, PlayerData target = null, CardInstance instance = null, EffectContext context = null)
    {
        if (user == null || target == null || instance == null) return;

        // 스택 드로우 스킵 플래그
        target.skipStackDraw = true;

        // 스택에서 제거
        StackManager.Instance.RemoveFromStack(instance);

        // 패로 복귀
        target.AddCardToHand(instance);
        GameActionController.Instance.MoveCardPublic(instance, CardZone.Hand);
        instance.isFaceUp = true;

        EventBus.Publish(new CardReturnedEvent{player = target, instance = instance});

        Debug.Log($"{user.playerName}이(가) 스택 드로우를 포기하고 엠프리스를 회수했습니다.");
    }
}