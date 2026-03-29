using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "CardEffects/TriggerContainer")]
public class TriggerContainer : CardEffect
{
    public enum TriggerType 
    { 
        OnBeforeDamage, 
        OnLowStackResolved,
        OnDamageResolved,
        OnTurnStart,
        OnStackPlayed
    }
    public TriggerType triggerType;
    public bool oneShot = false;
    public List<CardEffect> effectsToRun = new();

    public override void OnRegister(PlayerData user, CardInstance instance)
    {
        if (effectsToRun == null || effectsToRun.Count == 0)
            return;
        switch (triggerType)
        {
            case TriggerType.OnBeforeDamage:
                RegisterBeforeDamage(user, instance);
                break;

            case TriggerType.OnLowStackResolved:
                RegisterLowStack(user, instance);
                break;

            case TriggerType.OnDamageResolved:
                RegisterDamageResolved(user, instance);
                break;

            case TriggerType.OnTurnStart:
                RegisterTurnStart(user, instance);
                break;
            case TriggerType.OnStackPlayed:
                RegisterStackPlayed(user, instance);
                break;
        }
    }

    void RegisterBeforeDamage(PlayerData user, CardInstance instance)
    {
        Action<DamageCalculationEvent> handler = null;

        handler = (e) =>
        {
            if (e.Target != user)
                return;
            ExecuteEffects(user, instance, ref e.Damage);

            if (oneShot)
                EventBus.Unsubscribe(handler);
        };

        EventBus.Subscribe(handler);

        if (!oneShot)
            instance.AddUnregisterAction(() => EventBus.Unsubscribe(handler));
    }

    void RegisterLowStack(PlayerData user, CardInstance instance)
    {
        Action<LowStackResolvedEvent> handler = null;

        handler = (e) =>
        {
            if (e.Target != user)
                return;

            ExecuteEffects(user, e.Target, instance);

            if (oneShot)
                EventBus.Unsubscribe(handler);
        };

        EventBus.Subscribe(handler);

        if (!oneShot)
            instance.AddUnregisterAction(() => EventBus.Unsubscribe(handler));
    }

    void RegisterDamageResolved(PlayerData user, CardInstance instance)
    {
        Action<DamageResolvedEvent> handler = null;

        handler = (e) =>
        {
            ExecuteEffects(user, e.Target, instance);

            if (oneShot)
                EventBus.Unsubscribe(handler);
        };

        EventBus.Subscribe(handler);

        if (!oneShot)
            instance.AddUnregisterAction(() => EventBus.Unsubscribe(handler));
    }

    void RegisterTurnStart(PlayerData user, CardInstance instance)
    {
        Action<TurnStartEvent> handler = null;

        handler = (e) =>
        {
            if (e.Player != user)
                return;

            ExecuteEffects(user, user, instance);

            if (oneShot)
                EventBus.Unsubscribe(handler);
        };

        EventBus.Subscribe(handler);

        if (!oneShot)
            instance.AddUnregisterAction(() => EventBus.Unsubscribe(handler));
    }
    void RegisterStackPlayed(PlayerData user, CardInstance instance)
    {
        Action<StackCardPlayedEvent> handler = null;

        handler = (e) =>
        {
            // 자기 카드일 때만 발동
            if (e.instance != instance)
                return;
            ExecuteEffects(user, user, instance);

            if (oneShot)
                EventBus.Unsubscribe(handler);
        };
        EventBus.Subscribe(handler);

        if (!oneShot)
            instance.AddUnregisterAction(() => EventBus.Unsubscribe(handler));
    }
    void ExecuteEffects(PlayerData user, CardInstance instance, ref int damage)
    {
        foreach (var effect in effectsToRun)
        {
            if (effect == null)
                continue;

            if (effect is TriggerContainer)
                continue;

            effect.ModifyDamage(ref damage);
            effect.Execute(user, null, instance);
        }
    }

    void ExecuteEffects(PlayerData user, PlayerData target, CardInstance instance)
    {
        foreach (var effect in effectsToRun)
        {
            if (effect == null)
                continue;

            if (effect is TriggerContainer)
                continue;

            effect.Execute(user, target, instance);
        }
    }

    public override void Execute(PlayerData user, PlayerData target = null, CardInstance instance = null, EffectContext context = null)
    {
        // TriggerContainer는 직접 실행되지 않음
    }
}