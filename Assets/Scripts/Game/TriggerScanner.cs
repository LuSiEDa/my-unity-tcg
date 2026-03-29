using System;
using System.Collections;
using System.Collections.Generic;
public static class TriggerScanner
{
    public static List<CardInstance> FindValidTricks(PlayerData player, object evt)
    {
        List<CardInstance> validList = new List<CardInstance>();

        // 1. 현재 플레이어에게 세팅된 trickCard가 있는지 확인
        if (player.trickCard != null)
        {
            // 2. (선택 사항) 여기에 해당 카드가 현재 상황(evt)에 발동 가능한지 조건을 검사하는 로직을 넣습니다.
            // 지금은 테스트용이므로 있다면 무조건 리스트에 추가합니다.
            validList.Add(player.trickCard);
        }

        return validList;
    }

    // Counter(반격) 또한 비슷한 방식으로 검사할 수 있습니다.
    public static List<CardInstance> FindValidCounters(PlayerData player, object evt)
    {
        List<CardInstance> validList = new List<CardInstance>();

        // 예: 손패(hand)에 있는 카드 중 '카운터' 속성이 있는 카드를 찾거나,
        // 특정 구역에 있는 카드를 검사하는 로직이 들어갈 자리입니다.
        
        return validList;
    }
}