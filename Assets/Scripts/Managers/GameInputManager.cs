using UnityEngine;
using System;
using System.Collections.Generic;

public enum InputMode
{
    Normal,
    SelectingPlayer
}
public class GameInputManager : MonoBehaviour
{
    public static GameInputManager Instance;
    private void Awake() => Instance = this;
    private readonly List<PlayerUI> registeredPlayers = new();
    private Action<PlayerData> onPlayerSelected; // 플레이어가 선택되면 실행할 함수 저장

    public InputMode currentMode {get; private set;} = InputMode.Normal;

    // playerUI 등록
    public void RegisterPlayerUI(PlayerUI ui)
    {
        if (!registeredPlayers.Contains(ui))
            registeredPlayers.Add(ui);
    }
    public void UnregisterPlayerUI(PlayerUI ui)
    {
        registeredPlayers.Remove(ui);
    }

    // 플레이어 선택 시작
    public void StartSelectingPlayer(Action<PlayerData> callback)
    {
        if (currentMode != InputMode.Normal)
            return;

        currentMode = InputMode.SelectingPlayer;
        onPlayerSelected = callback;

        UIMessage.Instance.Show("플레이어를 선택하세요");
        ToggleAllPlayerSelection(true);
    }
    // PlayerUI가 호출하는 함수
    public void TrySelectPlayer(PlayerData player)
    {
        // 카드 Effect 선택 UI가 켜져있으면
        if (TargetSelectionUI.Instance != null)
        {
            TargetSelectionUI.Instance.SelectPlayer(player);
        }
        
        if (currentMode != InputMode.SelectingPlayer)
            return;
        onPlayerSelected?.Invoke(player);
        ExitSelectionMode();
    }
    private void ExitSelectionMode()
    {
        currentMode = InputMode.Normal;
        onPlayerSelected = null;

        UIMessage.Instance.Hide();
        ToggleAllPlayerSelection(false);
    }
    private void ToggleAllPlayerSelection(bool value)
    {
        foreach (var ui in registeredPlayers)
            ui.SetSelectable(value);
    }
}
