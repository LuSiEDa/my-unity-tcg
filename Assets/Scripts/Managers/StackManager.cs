using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StackManager : MonoBehaviour
{
    public static StackManager Instance;

    private readonly List<CardInstance> stack = new();

    public bool IsStackOpen => stack.Count > 0;
    public CardInstance TopCard => IsStackOpen ? stack[^1] : null;
    public PlayerData TopUser => TopCard?.user;

    private bool damageIncoming = false;
    private bool lowStackActive = false;

    private void Awake()
    {
        Instance = this;
    }

    // 스택 카드 스택
    public bool PlayStackCard(CardInstance instance)
    {
        if (instance == null || !instance.CanStack())
            return false;

        if (!IsStackOpen)
        {
            AddToStack(instance);
            GameActionController.Instance.Execute(
                instance.user,
                ActionType.DrawCard,
                new ActionContext(false, DrawReason.StackStart)
            );
            return true;
        }

        int topPower = TopCard.BasePower;
        int newPower = instance.BasePower;

        if (newPower >= topPower)
            AddToStack(instance);
        else
            ActionQueue.Enqueue(ResolveLowStack(instance));

        return true;
    }

    // 스택 추가
    private void AddToStack(CardInstance instance)
    {
        stack.Add(instance);
        GameActionController.Instance.MoveCardPublic(instance, CardZone.Stack);
        // 공격 트리거 핵심
        EventBus.Publish(new StackCardPlayedEvent(instance));

        LogManager.Instance.AddLog(
            $"{instance.user.playerName} 이(가) {instance.origin.cardName} 스택!",
            LogType.Stack
        );
    }

    // 일반 스택 해결
    public IEnumerator ResolveNormalStack()
    {
        if (!IsStackOpen) yield break;

        var top = TopCard;
        if (top == null)
        {
            ClearStack();
            yield break;
        }

        var target = top.target;
        if (target == null)
        {
            Debug.LogError("Stack target missing");
            ClearStack();
            yield break;
        }

        yield return GameActionController.Instance.ExecuteStackEffects(top);
        damageIncoming = true;
        // --- [수정 포인트: DamageCalculationEvent 적용] ---
        int baseDamage = top.BasePower;
        var damageEvt = new DamageCalculationEvent(
            top.target, 
            baseDamage,
            DamageSourceType.Stack,
            top);
        // 전국의 TriggerContainer들에게 데미지 수정을 요청함
        EventBus.Publish(damageEvt);

        yield return GameActionController.Instance.StartCoroutine(
            GameActionController.Instance.CheckTrick(target)
        );

        yield return null;

        int finalDamage = damageEvt.Damage;

        // 최종 수정된 데미지 적용
        target.ModifyLife(-finalDamage);
        damageIncoming = false;

        EventBus.Publish(new DamageResolvedEvent
        {
            Target = target,
            FinalDamage = finalDamage,
            Source = top
        });

        ClearStack();
    }

    // 로우 스택 처리
    private IEnumerator ResolveLowStack(CardInstance lowInstance)
    {
        lowStackActive = true;

        var top = TopCard;
        if (top == null)
            yield break;
        GameActionController.Instance.MoveCardPublic(lowInstance, CardZone.LowStack);

        int topPower = top.BasePower;
        int lowPower = lowInstance.BasePower;
        // --- [수정 포인트: LowStack 결과 이벤트 알림] ---
        // TriggerContainer의 OnLowStackResolved 트리거를 위해 발생시킴
        EventBus.Publish(new LowStackResolvedEvent {Target = lowInstance.user, ResolvedCard = lowInstance});
        
        GameActionController.Instance.Execute(
            lowInstance.user,
            ActionType.DrawCard,
            new ActionContext(false, DrawReason.StackHit)
        );
        yield return new WaitForSeconds(0.6f);
        yield return GameActionController.Instance.ExecuteStackEffects(lowInstance);

        int baseDamage = Mathf.Max(0, topPower - lowPower);
        
        if (lowInstance.target != null)
        {
            var target = lowInstance.user;
            // 마찬가지로 데미지 계산 이벤트를 거침
            var damageEvt = new DamageCalculationEvent(
                target, 
                baseDamage,
                DamageSourceType.Stack,
                lowInstance
                );
            EventBus.Publish(damageEvt);
            int finalDamage = damageEvt.Damage;
            target.ModifyLife(-finalDamage);
            EventBus.Publish(new DamageResolvedEvent
            {
                Target = target,
                FinalDamage = finalDamage,
                Source = lowInstance
            });
        }
        GameActionController.Instance.MoveCardPublic(lowInstance, CardZone.Discard);

        lowStackActive = false;
        ClearStack();
    }

    private void ClearStack()
    {
        foreach (var instance in stack)
        {
            if (instance.currentZone != CardZone.Stack) continue;

            GameActionController.Instance.MoveCardPublic(instance, CardZone.Discard);
        }

        stack.Clear();
    }

    // 유틸
    public bool IsInStack(CardInstance instance)
    {
        return stack.Contains(instance);
    }

    public void RemoveFromStack(CardInstance instance)
    {
        if (instance == null) return;
        stack.Remove(instance);
    }

    public void ForceClearAllStacks()
    {
        foreach (var card in stack)
        {
            GameActionController.Instance.MoveCardPublic(card, CardZone.Discard);
        }
        stack.Clear();
    }
    public bool HasLowStack()
    {
        return lowStackActive;
    }
    public bool IsDamageIncoming()
    {
        return damageIncoming;
    }
}