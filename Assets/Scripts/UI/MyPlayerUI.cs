using UnityEngine;

public class MyPlayerUI : PlayerUI
{
    [Header("My Only")]
    [SerializeField]
    private HandUI handUI;

    public override void Setup(PlayerData playerData)
    {
        // 씬에 이미 배치된 HandUI를 확실히 찾습니다.
        if (handUI == null) handUI = GetComponentInChildren<HandUI>();
        
        if (handUI != null) handUI.Setup(playerData);

        base.Setup(playerData); // 여기서 Event 구독이 일어남
    }
    public override void Refresh()
    {
        base.Refresh();
        if (handUI != null) handUI.Refresh();
    }
}
