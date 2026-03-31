using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameState CurrentState { get; private set; }
    public static void SetState(GameState state)
    {
        if (CurrentState == state) return; // 같은 상태로 변경 시 무시
        CurrentState = state;
        Debug.Log($"GameState -> {state}");
    }
    public static bool IsWaitingInput()
    {
        return CurrentState == GameState.WaitingForTrick 
            || CurrentState == GameState.WaitingForCounter;
    }
}
