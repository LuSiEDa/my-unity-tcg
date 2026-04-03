using System.Collections;
using UnityEngine;

public class GameActionController : MonoBehaviour
{
    public static GameActionController Instance;
    private void Awake() => Instance = this;

    // 외부 실행 진입점
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

        user.trickCheckedThisWindow = false;
        yield return StartCoroutine(CheckTrick(user));
    }
private EffectContext BuildContext(CardInstance instance, ActionType action)
    {
        EffectContext context = new EffectContext();

        context.source = instance?.user;

        // 전부 초기화
        context.isImmediate = false;
        context.isStackPlayed = false;
        context.isDamageIncoming = false;
        context.isLowStack = false;
        context.isDamageResolved = false;
        context.isTurnStart = false;
        context.isTurnEnd = false;

        // 행동 기반
        switch (action)
        {
            case ActionType.UseCard:
            case ActionType.OpenTrick:
                context.isImmediate = true;
                break;

            case ActionType.StackAttack:
                context.isStackPlayed = true;
                break;
        }

        // 상태 기반 (보조 정보)
        if (StackManager.Instance.IsDamageIncoming())
            context.isDamageIncoming = true;

        if (StackManager.Instance.HasLowStack())
            context.isLowStack = true;

        if (TurnManager.Instance.CurrentPhase == TurnPhase.Start)
            context.isTurnStart = true;

        if (TurnManager.Instance.CurrentPhase == TurnPhase.End)
            context.isTurnEnd = true;

        return context;
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
        yield return ExecuteCardEffects(instance, ActionType.UseCard);

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
        
        if (newCard.origin?.effects != null)
        {
            foreach (var effect in newCard.origin.effects)
            {
                effect.OnRegister(target, newCard);
            }
        }

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

        yield return ExecuteCardEffects(instance, ActionType.OpenTrick);

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

    private IEnumerator ExecuteCardEffects(CardInstance instance, ActionType action)
    {
        if (instance?.origin?.effects == null) yield break;

        EffectContext context = BuildContext(instance, action);

        foreach (var effect in instance.origin.effects)
        {
            if (effect == null) continue;
            // 1. 트리거 컨테이너인 경우 (조건부 효과)
            if (effect is TriggerContainer tc)
            {
                // 지금 상황(context)이 트리거 조건과 맞는지 확인
                if (!tc.MatchTiming(context)) 
                    continue; // 안 맞으면 이 컨테이너는 건너뜀

                // 맞으면 그 안에 들어있는 진짜 효과들을 실행
                foreach (var subEffect in tc.effectsToRun)
                {
                    if (subEffect == null) continue;
                    
                    PlayerData subSelectedTarget = null; // 트리거 내부용 선택 타겟

                    // 트리거 내부 효과가 선택을 요구할 때
                    if (subEffect.RequiresSelection())
                    {
                        bool done = false;
                        TargetSelectionUI.Instance.StartSelection(player => {
                            subSelectedTarget = player; 
                            done = true; 
                        });
                        while (!done) yield return null;
                    }

                    // 수정 포인트: null 대신 subSelectedTarget을 전달해야 함!
                    var subTargets = TargetResolver.ResolvePlayers(subEffect.targetType, instance.user, subSelectedTarget);
                    foreach (var t in subTargets)
                    {
                        context.target = t;
                        subEffect.Execute(instance.user, t, instance, context);
                    }
                }
                continue; 
            }

            // 2. 일반 효과 로직
            if (effect.timing == CardTiming.StackOnly) continue;
            if (!CheckTiming(effect.timing)) continue;

            PlayerData selectedTarget = null; // 일반 효과용 선택 타겟

            if (effect.RequiresSelection())
            {
                bool done = false;
                TargetSelectionUI.Instance.StartSelection(player => {
                    selectedTarget = player; // 아까 오타났던 부분 (commonSelectedTarget -> selectedTarget)
                    done = true; 
                });
                while (!done) yield return null;
            }
            
            var targets = TargetResolver.ResolvePlayers(
                effect.targetType,
                instance.user,
                selectedTarget // 아까 오타났던 부분
            );

            foreach (var t in targets)
            {
                context.target = t;
                effect.Execute(instance.user, t, instance, context);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    public IEnumerator ExecuteStackEffects(CardInstance instance)
    {
        if (instance?.origin?.effects == null)
            yield break;
        
        EffectContext context = BuildContext(instance, ActionType.StackAttack);

        foreach (var effect in instance.origin.effects)
        {
            if (effect == null) continue;
            if (effect is TriggerContainer)
                continue;
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
    // 기다리는 쪽
    public IEnumerator CheckTrick(PlayerData player)
    {
        if (player.trickCheckedThisWindow || player.trickCard == null || player.trickCard.isFaceUp)
            yield break;

        var trick = player.trickCard;
        bool canTrigger = false;
        
        EffectContext context = BuildContext(trick, ActionType.OpenTrick);
        context.source = player;

        context.isImmediate = false;
        context.isStackPlayed = false;

        context.isDamageIncoming = StackManager.Instance.IsDamageIncoming();
        context.isLowStack = StackManager.Instance.HasLowStack();
        context.isTurnStart = TurnManager.Instance.CurrentPhase == TurnPhase.Start;
        context.isTurnEnd = TurnManager.Instance.CurrentPhase == TurnPhase.End;

        foreach (var effect in trick.origin.effects)
        {
            if (effect is TriggerContainer tc)
            {
                if (tc.MatchTiming(context))
                {
                    canTrigger = true;
                    break;
                }
            }
            // 일반 효과가 트릭에 섞여 있을 경우를 대비해 타이밍 체크 로직 유지 가능
            else if (effect != null && CheckTiming(effect.timing))
            {
                canTrigger = true;
                break;
            }
        }
        if (!canTrigger)
            yield break;
        // 중복 방지
        player.trickCheckedThisWindow = true;
        
        // 결정 상태 관리 변수
        bool decided = false;
        CardInstance selectedInstance = null;

        System.Action<TrickSelectedEvent> handler = null;
        handler = (e) =>
        {
            if (e.player != player) return;
            selectedInstance = e.instance;
            decided = true;
        };
        EventBus.Subscribe(handler);

        float timeout = 3f;
        EventBus.Publish(new TrickDecisionEvent(player,trick, timeout));

        float timer = 0f;
        while (!decided && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        EventBus.Unsubscribe(handler);

        // UI 강제 종료 이벤트
        EventBus.Publish(new TrickSelectedEvent(player,null));
        if (decided && selectedInstance != null)
        {
            yield return ExecuteTrickDirect(player,trick);
        }
        else
        {
            Debug.Log($"[트릭 패스 또는 타임아웃] {trick.origin.cardName}");
        }
    }

}