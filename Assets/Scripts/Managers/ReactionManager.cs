using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class ReactionManager
{
    private const int MAX_DEPTH = 10;

    public static IEnumerator RunReaction(object evt)
    {
        int depth = 0;

        while (true)
        {
            depth++;
            if (depth > MAX_DEPTH)
            {
                Debug.LogWarning("Reaction depth limit");
                yield break;
            }

            GameStateManager.SetState(GameState.WaitingForTrick);
            List<TrickResponse> responses = null;

            yield return CollectResponses(evt, r => responses = r);
            if (responses == null || responses.Count == 0)
                break;
            
            GameStateManager.SetState(GameState.ResolvingTrick);

            foreach (var r in responses)
            {
                yield return ExecuteTrick(r);
            }
        }
        GameStateManager.SetState(GameState.Idle);
    }
    static IEnumerator ExecuteTrick(TrickResponse r)
    {
        yield return GameActionController.Instance.ExecuteTrickDirect(r.player, r.card);
    }

    // 누군가 카드를 냈을 때, 다른 사람들이 대응할 시간을 주고 그 결과를 기다림
    static IEnumerator CollectResponses(object evt, Action<List<TrickResponse>> callback)
    {
        // 반응 초기화 및 대상 설정
        List<TrickResponse> responses = new();
        Dictionary<PlayerData, bool> responded = new();

        foreach (var p in TurnManager.Instance.players)
            responded[p] = false; // 모든 플레이어가 아직 응답하지 않았다고 표시
        // UI 요청 (비동기 호출)
        foreach (var player in TurnManager.Instance.players)
        {
            var valid = TriggerScanner.FindValidTricks(player, evt); // 낼 수 있는 카드 탐색

            UIManager.Instance.ShowTrickUI(player, valid, (card) =>
            {
                if (card != null)
                    responses.Add(new TrickResponse(player, card));
                
                responded[player] = true;
            }); // UI 띄우기
        }
        // 전원 응답 대기
        yield return new WaitUntil(() =>
        {
            foreach (var v in responded.Values)
                if (!v) return false; // 한 명이라도 false면 계속 기다림
            return true;
        });
        callback(responses);
    }
}
