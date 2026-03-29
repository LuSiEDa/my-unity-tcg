using System.Collections;
using UnityEngine;

public class GameActionController : MonoBehaviour
{
    public static GameActionController Instance;
    private void Awake() => Instance = this;

    public bool Execute(
        PlayerData user, 
        ActionType action,
        ActionContext context, 
        CardInstance instance = null, 
        PlayerData target = null)
    {
        if (user == null)
            return false;
        
        if (context.consumeActionPoint && user.actionPoint <= 0)
            return false;
        // 카드 규칙 검증 (단일 시스템)
        if (instance != null)
        {
            if (!CardRuleValidator.CanPlayCard(user, instance, action))
                return false;
        }
        ActionQueue.Enqueue(
            ActionSequence(user, action, context, instance, target)
        );
        return true;
    }

    private IEnumerator ActionSequence(
        PlayerData user, 
        ActionType action,
        ActionContext context, 
        CardInstance instance, 
        PlayerData target)
    {
        bool success = false;

        switch (action)
        {
            case ActionType.UseCard:
                yield return HandleUseCard(user, instance);
                success = true;
                break;

            case ActionType.StackAttack:
                yield return HandleStackAttack(user, instance);
                success = true;
                break;

            case ActionType.PlaceKeep:
                success = HandlePlaceKeep(user, target, instance);
                break;

            case ActionType.PlaceTrick:
                success = HandlePlaceTrick(user, instance);
                break;

            case ActionType.OpenTrick:
                yield return HandleOpenTrick(user, instance);
                success = true;
                break;

            case ActionType.DrawCard:
                success = HandleDraw(user, context);
                break;
        }

        if (!success)
            yield break;
        if (context.consumeActionPoint)
        {
            user.ModifyActionPoint(-1);
            EventBus.Publish(new ActionPointChangedEvent(user, user.actionPoint));
        }
        yield return new WaitForSeconds(0.2f);
    }
    public static bool CheckTiming(CardTiming timing)
    {
        switch (timing)
        {
            case CardTiming.Anytime:
                return true;
            case CardTiming.StackOnly:
                return StackManager.Instance.IsStackOpen;
            case CardTiming.LowStack:
                return StackManager.Instance.HasLowStack();
            case CardTiming.DamageIncoming:
                return StackManager.Instance.IsDamageIncoming();
            case CardTiming.TurnStart:
                return TurnManager.Instance.CurrentPhase == TurnPhase.Start;
            case CardTiming.TurnEnd:
                return TurnManager.Instance.CurrentPhase == TurnPhase.End;
        }
        return true;
    }

    private IEnumerator HandleUseCard(PlayerData user, CardInstance instance)
    {
        if (instance == null || !instance.CanUse())
            yield break;

        user.RemoveCardFromHand(instance);
        MoveCard(instance, CardZone.Use);

        EventBus.Publish(new CardUsedEvent(user, instance));
        yield return ExecuteCardEffects(instance);

        MoveCard(instance, CardZone.Discard);
    }

    private IEnumerator HandleStackAttack(PlayerData user, CardInstance instance)
    {
        if (instance == null || !instance.CanStack())
            yield break;
        
        if (instance.target == null)
        {
            instance.target = TargetResolver.ResolvePlayers(
                TargetType.Opponent,
                user
            )[0];
        }
        
        if (!StackManager.Instance.PlayStackCard(instance))
            yield break;

        user.RemoveCardFromHand(instance);
    }

    private bool HandlePlaceKeep(PlayerData user, PlayerData target, CardInstance newCard)
    {
        if (target == null || newCard == null || !newCard.CanKeep())
            return false;

        if (target.activeKeepCard != null)
        {
            MoveCard(target.activeKeepCard, CardZone.Discard);
        }

        user.RemoveCardFromHand(newCard);
        target.SetKeepCard(newCard);

        MoveCard(newCard, CardZone.Keep);
        // --- 지속 설치시 효과가 있다면 처리 ---
        if (newCard.origin?.effects != null)
        {
            foreach (var effect in newCard.origin.effects)
            {
                effect.OnRegister(target, newCard);
            }
        }
        // --- 실제로 유용한가는 두고봐야겠지 ---
        EventBus.Publish(new PlaceKeepEvent(user, target, newCard));
        return true;
    }

    private bool HandlePlaceTrick(PlayerData user, CardInstance instance)
    {
        if (instance == null || !instance.CanTrick())
            return false;

        user.RemoveCardFromHand(instance);
        user.SetTrickCard(instance);

        instance.isFaceUp = false;
        MoveCard(instance, CardZone.Trick);
        
        EventBus.Publish(new TrickPlacedEvent(user));
        return true;
    }

    private IEnumerator HandleOpenTrick(PlayerData user, CardInstance instance)
    {
        if (instance == null || user.trickCard != instance)
            yield break;

        instance.isFaceUp = true;

        EventBus.Publish(new TrickOpenedEvent(user, instance));

        yield return ExecuteCardEffects(instance);

        user.SetTrickCard(null);
        MoveCard(instance, CardZone.Discard);
    }

    private bool HandleDraw(PlayerData user, ActionContext context)
    {
        var drawn = DeckManager.Instance.Draw(user);
        if (drawn == null)
            return false;

        user.AddCardToHand(drawn);
        drawn.isFaceUp = true;

        EventBus.Publish(new CardDrawnEvent(user, drawn, context.drawReason));
        return true;
    }

    // =========================

    private IEnumerator ExecuteCardEffects(CardInstance instance)
    {
        if (instance?.origin?.effects == null) yield break;

        EffectContext context = new EffectContext();

        foreach (var effect in instance.origin.effects)
        {
            if (effect == null) continue;

            // 스택 전용 효과는 여기서 실행 금지
            if (effect.timing == CardTiming.StackOnly)
                continue;

            // 타이밍 검사
            if (!CheckTiming(effect.timing))
                continue;
            
            PlayerData selectedTarget = null;

            if (effect.RequiresSelection())
            {
                bool done = false;

                TargetSelectionUI.Instance.StartSelection(player =>
                {
                    selectedTarget = player;
                    done = true;
                });
                // 플레이이어가 선택할 때까지 대기
                while (!done)
                    yield return null;
            }
            
            // TargetType 기반 타겟 계산
            var targets = TargetResolver.ResolvePlayers(
                effect.targetType,
                instance.user,
                selectedTarget
            );
            // 여러 타겟 실행
            foreach (var t in targets)
            {
                effect.Execute(instance.user, t, instance, context);
            }
            // 효과 사이의 아주 짧은 연출 대기 (필요시)
            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator ExecuteStackEffects(CardInstance instance)
    {
        if (instance?.origin?.effects == null)
            yield break;
        
        EffectContext context = new EffectContext();

        foreach (var effect in instance.origin.effects)
        {
            if (effect == null) continue;
            // StackOnly만 실행
            if (effect.timing != CardTiming.StackOnly)
                continue;
            if (!CheckTiming(effect.timing))
                continue;
            
            var targets = TargetResolver.ResolvePlayers(
                effect.targetType,
                instance.user,
                instance.target
            );
            foreach (var t in targets)
            {
                effect.Execute(instance.user, t, instance, context);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void MoveCard(CardInstance instance, CardZone zone)
    {
        var oldZone = instance.currentZone;
        if (oldZone == CardZone.Keep)
        {
            instance.ClearAllRegisteredEvents();
        }
        instance.currentZone = zone;
        EventBus.Publish(new CardMovedEvent(instance, zone));
    }


    // 지연 삭제
    private IEnumerator SendToDiscardDelayed(CardInstance instance, float delay)
    {
        yield return new WaitForSeconds(delay);

        MoveCard(instance, CardZone.Discard);
    }
    // 외부 접근용
    public void MoveCardPublic(CardInstance instance, CardZone newZone)
    {
        MoveCard(instance, newZone);
    }
    // 지속 설치
    public void MoveCardToKeep(CardInstance instance, PlayerData target, bool isForced)
    {
        if (target.activeKeepCard != null)
        {
            MoveCard(target.activeKeepCard,CardZone.Discard);
        }
        target.SetKeepCard(instance);
        MoveCard(instance, CardZone.Keep);
        if (instance.origin?.effects != null)
        {
            foreach (var effect in instance.origin.effects)
            {
                effect.OnRegister(target, instance);
            }
        }
    }
    public IEnumerator ExecuteTrickDirect(PlayerData user, CardInstance instance)
    {
        yield return HandleOpenTrick(user, instance);
    }
}