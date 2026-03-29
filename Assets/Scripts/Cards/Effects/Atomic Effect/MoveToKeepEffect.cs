using UnityEngine;

[CreateAssetMenu(fileName = "MoveToKeepEffect", menuName = "CardEffects/MoveToKeepEffect")]
public class MoveToKeepEffect : CardEffect
{
    // 인스펙터에서 체크 가능. true면 행동력 소모 없이 이동.
    public bool isForced = true; 

    // ★중요: 아래 Execute 함수는 이 파일 안에 딱 '하나'만 있어야 합니다!
    public override void Execute(PlayerData user, PlayerData target, CardInstance instance, EffectContext context = null)
    {
        if (instance == null || target == null) return;
        
        if (!StackManager.Instance.IsInStack(instance)) return;
        
        StackManager.Instance.RemoveFromStack(instance);

        GameActionController.Instance.MoveCardToKeep(instance, target, isForced);
    }
    
}