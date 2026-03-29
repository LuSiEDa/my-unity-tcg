using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CounterManager
{
    public static IEnumerator OpenCounterWindow(CardInstance target, Action<bool> callback)
    {
        GameStateManager.SetState(GameState.WaitingForCounter);

        List<CardInstance> counters = new();
        
        foreach (var player in TurnManager.Instance.players)
        {
            var valid = FindCounters(player, target);

            if (valid.Count > 0)
            {
                bool done = false;

                UIManager.Instance.ShowCounterUI(player, valid, (card) =>
                {
                    if (card != null)
                        counters.Add(card);
                    
                    done = true;
                });
                while (!done)
                    yield return null;
            }
        }
        // 역순 실행
        for (int i = counters.Count - 1; i >= 0; i--)
        {
            foreach (var counter in counters)
            {
                yield return GameActionController.Instance.ExecuteTrickDirect(counter.user, counter);
            }
        }
        callback(false);
    }
    
    static List<CardInstance> FindCounters(PlayerData player, CardInstance target)
    {
        return new List<CardInstance>(); // 일단 없음 처리
    }
}
