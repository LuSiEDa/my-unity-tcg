using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "CardEffects/TriggerContainer")]
public class TriggerContainer : CardEffect
{
    public enum TriggerType 
    { 
        Immediate,
        OnBeforeDamage, 
        OnLowStackResolved,
        OnDamageResolved,
        OnTurnStart,
        OnTurnEnd,
        OnStackPlayed
    }
    public TriggerType triggerType;
    public List<CardEffect> effectsToRun = new();

    public override void Execute(PlayerData user, PlayerData target = null, CardInstance instance = null, EffectContext context = null)
    {
        if (context == null)
            return;

        if (!MatchTiming(context))
            return;

        foreach (var effect in effectsToRun)
        {
            if (effect == null) continue;

            effect.Execute(user, target, instance, context);
        }
    }
    public bool MatchTiming(EffectContext context)
    {
        switch (triggerType)
        {
            case TriggerType.Immediate:
                return context.isImmediate;
            case TriggerType.OnBeforeDamage:
                return context.isDamageIncoming;
            case TriggerType.OnLowStackResolved:
                return context.isLowStack;
            case TriggerType.OnDamageResolved:
                return context.isDamageResolved;
            case TriggerType.OnTurnStart:
                return context.isTurnStart;
            case TriggerType.OnTurnEnd:
                return context.isTurnEnd;
            case TriggerType.OnStackPlayed:
                return context.isStackPlayed;
        }
        return false;
    }
}