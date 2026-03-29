using UnityEngine;
using System;
using System.Collections.Generic;

public class CardInstance
{
    public PlayerData user;      // 카드 소유자
    public Card origin;          // 카드 데이터

    public bool isFaceUp = false;
    public CardZone currentZone;
    public PlayerData target;

    private readonly List<Action> onUnregisterActions = new List<Action>();

    // 생성자: card + user 그대로
    public CardInstance(Card instance, PlayerData user)
    {
        origin = instance;
        this.user = user;
        currentZone = CardZone.Hand;
    }
    // 새로운 구독 정보를 장부에 추가
    public void AddUnregisterAction(Action unregisterAction)
    {
        onUnregisterActions.Add(unregisterAction);
    }
    // 카드가 파괴되거나 효과가 끝날 때 호출 => 모든 구독을 지움
    public void ClearAllRegisteredEvents()
    {
        foreach (var action in onUnregisterActions)
        {
            action?.Invoke();
        }
        onUnregisterActions.Clear();
    }


    // ===== 상태 판별 =====
    public bool CanUse() =>
        origin != null &&
        origin.HasCategory(CardCategory.Use) &&
        currentZone == CardZone.Hand;

    public bool CanKeep() =>
        origin != null &&
        origin.HasCategory(CardCategory.Keep) &&
        currentZone == CardZone.Hand;

    public bool CanTrick() =>
        origin != null &&
        origin.HasCategory(CardCategory.Trick) &&
        currentZone == CardZone.Hand;

    public bool CanStack()
    {
        if (origin == null) return false;
        if (currentZone != CardZone.Hand) return false;

        if (StackManager.Instance.IsStackOpen) return true;
        
        return origin.basePower != 0;
    }

    public int BasePower => origin != null ? origin.basePower : 0;
}