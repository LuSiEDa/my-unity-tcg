using UnityEngine;
using System.Collections.Generic;

public static class TargetResolver
{
    public static List<PlayerData> ResolvePlayers(
        TargetType type,
        PlayerData user,
        PlayerData explicitTarget = null)
    {
        List<PlayerData> result = new List<PlayerData>();
        List<PlayerData> players = TurnManager.Instance.players;

        switch (type)
        {
            case TargetType.None:
                break;
            case TargetType.Self:
                result.Add(user);
                break;
            case TargetType.Opponent:
                result.Add(GetNextOpponent(user));
                break;
            case TargetType.AllPlayers:
                result.AddRange(players);
                break;
            case TargetType.AllOpponents:
                foreach (var p in players)
                    if (p != user)
                        result.Add(p);
                break;
            case TargetType.RandomPlayer:
                result.Add(players[Random.Range(0, players.Count)]);
                break;

            case TargetType.RandomOpponent:
                List<PlayerData> opponents = new List<PlayerData>();

                foreach (var p in players)
                    if (p != user)
                        opponents.Add(p);

                result.Add(opponents[Random.Range(0, opponents.Count)]);
                break;
            case TargetType.ChoosePlayer:
                if (explicitTarget != null)
                    result.Add(explicitTarget);
                break;
            case TargetType.LeftPlayer:
                result.Add(GetLeftPlayer(user));
                break;

            case TargetType.RightPlayer:
                result.Add(GetRightPlayer(user));
                break;

            case TargetType.AdjacentPlayers:
                result.Add(GetLeftPlayer(user));
                result.Add(GetRightPlayer(user));
                break;
        }
        return result;
    }
    
    // 다음 플레이어 조회
    private static PlayerData GetNextOpponent(PlayerData user)
    {
        var tm = TurnManager.Instance;
        List<PlayerData> players = tm.players;
        int currentIndex = players.IndexOf(user);
        int next = tm.isClockwise
            ? (currentIndex + 1) % players.Count
            : (currentIndex - 1 + players.Count) % players.Count;

        return players[next];
    }
    private static PlayerData GetLeftPlayer(PlayerData user)
    {
        var tm = TurnManager.Instance;
        List<PlayerData> players = tm.players;

        int index = players.IndexOf(user);
        int left = (index - 1 + players.Count) % players.Count;

        return players[left];
    }

    private static PlayerData GetRightPlayer(PlayerData user)
    {
        var tm = TurnManager.Instance;
        List<PlayerData> players = tm.players;

        int index = players.IndexOf(user);
        int right = (index + 1) % players.Count;

        return players[right];
    }
}
