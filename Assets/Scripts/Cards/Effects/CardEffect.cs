using UnityEngine;

[CreateAssetMenu(fileName = "CardEffect", menuName = "Game/CardEffect")]
public abstract class CardEffect : ScriptableObject
{
    public TargetType targetType;
    public CardTiming timing = CardTiming.Anytime;
    // 타겟 필요 여부 정의
    public virtual bool RequiresSelection()
    {
        return targetType == TargetType.ChoosePlayer;
    }
    // 모든 카드 효과는 Execute라는 기능을 가져야 한다. 라는 규칙을 정해둠.
    public abstract void Execute(PlayerData user, PlayerData target = null, CardInstance instance = null, EffectContext context = null);

    // 필요 시 데미지 수정
    public virtual void ModifyDamage(ref int damage)
    {
       
    }

    // 이벤트 구독 등록
    public virtual void OnRegister(PlayerData user, CardInstance instance)
    {
       
    }

    // 이벤트 구독 해제
    public virtual void OnUnregister(PlayerData usr, CardInstance instance)
    {
       
    }

    public virtual void Deactivate(PlayerData user, CardInstance instance)
    {
        // 필요시 오버라이드
    }
}