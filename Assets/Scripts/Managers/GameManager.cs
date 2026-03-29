using UnityEngine;
using System.Collections.Generic;

public enum GameInit
    {
        Waiting,
        Initializing,// 플레이어 생성 중
        InGame,      // 실제 게임 플레이
        GameOver
    }
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameInit CurrentState;
    public int playerCount = 4;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeGame();
    }
    private void InitializeGame()
    {
        ChangeState(GameInit.Initializing);
        // 플레이어 생성
        List<PlayerData> players = new List<PlayerData>();

        for (int i = 0; i < playerCount; i++)
        {
            players.Add(new PlayerData("Player " + (i + 1), i));
        }
        PlayerData localPlayer = players[0];
        // TurnManager에 전달
        TurnManager.Instance.Initialize(players, localPlayer);
        // 게임 시작
        TurnManager.Instance.StartGame();
        ChangeState(GameInit.InGame);
    }
    // 상태 제어 시스템 (게임 흐름의 단계 전환)
    public void ChangeState(GameInit newState)
    {
        CurrentState = newState;
        Debug.Log("State Changed To:" + newState);
    }
}
