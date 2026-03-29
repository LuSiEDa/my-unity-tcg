using UnityEngine;
using System;

public class TargetSelectionUI : MonoBehaviour
{
    // 카드 효과에서 플레이어를 선택할 때만 쓰는 전용 UI 컨트롤러.
    public static TargetSelectionUI Instance;
    private Action<PlayerData> onSelected;

    private void Awake()
    {
        Instance = this;
    }
    // 카드 effect가 플레이어 선택을 요청할 때 호출
    public void StartSelection(Action<PlayerData> callback)
    {
        onSelected = callback;
        UIMessage.Instance.Show("대상을 선택하세요");
        // 여기서 플레이어 UI 하이라이트 등 가능
        var players = FindObjectsByType<PlayerUI>(FindObjectsSortMode.None);

        foreach (var p in players)
            p.SetSelectable(true);
    }
    // PlayerUI 클릭 시 호출
    public void SelectPlayer(PlayerData player)
    {
        if (onSelected == null) return;

        var callback = onSelected;
        onSelected = null;
        // 선택 UI 끄기
        var players = FindObjectsByType<PlayerUI>(FindObjectsSortMode.None);
        foreach (var p in players)
            p.SetSelectable(false);
            
        UIMessage.Instance.Hide();
        callback.Invoke(player);
    }
}
