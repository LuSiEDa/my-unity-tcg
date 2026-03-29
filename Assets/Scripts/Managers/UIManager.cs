using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("Prefabs")]
    [SerializeField] private GameObject myBoardPrefab;
    [SerializeField] private GameObject enemyBoardPrefab;
    [SerializeField] private GameObject cardPrefab; //카드 Ui 프리팹

    [Header("Parents")]
    [SerializeField] private Transform myBoardArea;
    [SerializeField] private Transform topPlayersArea;
    
    [Header("Global Zones")]
    [SerializeField] private Transform stackZone;
    [SerializeField] private Transform lowStackZone;
    [SerializeField] private Transform useZone;
    [SerializeField] private Transform discardZone;

    private PlayerUI myUI;
    private List<PlayerUI> enemySlots = new List<PlayerUI>();
    private Dictionary<CardInstance, CardUI> cardUIMap = new();
    void Awake()
    {
        Instance = this;
    }
    void OnEnable()
    {
        EventBus.Subscribe<GameStartEvent>(OnGameStart);
        EventBus.Subscribe<CurrentPlayerChangedEvent>(OnTurnChanged);
        EventBus.Subscribe<CardMovedEvent>(OnCardMoved);
        EventBus.Subscribe<DiscardPileChangedEvent>(OnDiscardChanged);
    }
    void OnDisable()
    {
        EventBus.Unsubscribe<GameStartEvent>(OnGameStart);
        EventBus.Unsubscribe<CurrentPlayerChangedEvent>(OnTurnChanged);
        EventBus.Unsubscribe<CardMovedEvent>(OnCardMoved);
        EventBus.Unsubscribe<DiscardPileChangedEvent>(OnDiscardChanged);
    }
    // 이벤트 핸들러
    private void OnGameStart(GameStartEvent e)
    {
        CreateAllBoards(e.players, e.localPlayer);
    }
    private void OnTurnChanged(CurrentPlayerChangedEvent e)
    {
        HighlightCurrentPlayer(e.currentPlayer);
    }
    private void OnCardMoved(CardMovedEvent e)
    {
        MoveCardToZone(e.instance, e.targetZone);
    }
    private void OnDiscardChanged(DiscardPileChangedEvent e)
    {
        // discardCountText.text = e.count.ToString();
        // UpdateTopDiscardCard(e.topCard);
    }


    // UI 생성
    public void CreateAllBoards(List<PlayerData> players, PlayerData localPlayer)
    {
        ClearAll();

        CreateMyBoard(localPlayer);
        CreateEnemyBoards(players, localPlayer);
    }
    // 내 UI 생성
    void CreateMyBoard(PlayerData localPlayer)
    {
        GameObject board = Instantiate(myBoardPrefab, myBoardArea);
        RectTransform rect = board.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        myUI = board.GetComponent<PlayerUI>();
        myUI.Setup(localPlayer);

        GameInputManager.Instance.RegisterPlayerUI(myUI);
    }
    // 상대방 UI 생성
    public void CreateEnemyBoards(List<PlayerData> players, PlayerData localPlayer)
    {
        foreach (var player in players)
        {
            if (player == localPlayer)
                continue;

        GameObject slot = Instantiate(enemyBoardPrefab, topPlayersArea);
        RectTransform rect = slot.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        PlayerUI ui = slot.GetComponent<PlayerUI>();
        ui.Setup(player);

        GameInputManager.Instance.RegisterPlayerUI(ui);
        
        enemySlots.Add(ui);
        }
    }
    // UI 오브젝트 파괴
    void ClearAll()
    {
        if (myUI != null)
            Destroy(myUI.gameObject);
        
        foreach (var ui in enemySlots)
        {
            if (ui != null)
                Destroy(ui.gameObject);
        }
        enemySlots.Clear();
        foreach (var pair in cardUIMap)
        {
            if (pair.Value != null)
                Destroy(pair.Value.gameObject);
        }

        cardUIMap.Clear();
    }
    public void MoveCardToZone(CardInstance instance, CardZone zone)
    {
        if (instance == null) return;

        // instance.currentZone = zone; 삭제

        Transform parent = GetZoneTransform(zone, instance.user);
        if (parent == null) return;

        CardUI ui = GetOrCreateUI(instance);

        ui.transform.SetParent(parent, false);
        ui.UpdateVisual(GetSizeByZone(zone, instance.user));

        RectTransform rt = ui.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }
    }
    Transform GetZoneTransform(CardZone zone, PlayerData user)
    {
        switch (zone)
        {
            case CardZone.Stack:
                return stackZone;
            case CardZone.LowStack: 
                return lowStackZone;
            case CardZone.Use: 
                return useZone;
            case CardZone.Discard:
                return discardZone;
        }

        if (user == null)
            return null;

        if (myUI != null && user == myUI.Data)
            return myUI.GetZone(zone);

        foreach (var ui in enemySlots)
            if (ui.Data == user)
                return ui.GetZone(zone);

        return null;           
    }
    CardUI GetOrCreateUI(CardInstance instance)
    {
        if (cardUIMap.TryGetValue(instance, out var existingUI))
            return existingUI;
        
        GameObject obj = Instantiate(cardPrefab);
        CardUI ui = obj.GetComponent<CardUI>();
        ui.Setup(instance, GetSizeByZone(instance.currentZone, instance.user));

        cardUIMap.Add(instance, ui);
        return ui;
    }
    private CardUI.CardSize GetSizeByZone(CardZone zone, PlayerData user)
    {
        // 상대 보드의 Keep은 S
        if (zone == CardZone.Keep)
        {
            if (user != TurnManager.Instance.localPlayer)
                return CardUI.CardSize.S;
            return CardUI.CardSize.M;
        }
        switch (zone)
        {
            case CardZone.Discard:
            case CardZone.LowStack:
                return CardUI.CardSize.S;

            case CardZone.Stack:
            case CardZone.Hand:
            case CardZone.Use:
                return CardUI.CardSize.M;

            default:
                return CardUI.CardSize.M;
        }
    }
    // public void MoveStackToDiscard(CardZone fromZone)
    // {
    //     if (discardZone == null) return;

    //     foreach (var pair in cardUIMap)
    //     {
    //         if (pair.Key.currentZone == fromZone)
    //             MoveCardToZone(pair.Key, CardZone.Discard);
    //     }
    // }

    // public void ClearStackUI()
    // {
    //     foreach (var obj in stackCardUIs)
    //     {
    //         if (obj != null)
    //             Destroy(obj);
    //     }
    //     stackCardUIs.Clear();
    // }
    // public void ClearDiscardUI()
    // {
    //     foreach (var obj in discardUIs)
    //     {
    //         if (obj != null)
    //             Destroy(obj);
    //     }
    //     discardUIs.Clear();
    // }
    // 턴 플레이어 강조
    public void HighlightCurrentPlayer(PlayerData currentPlayer)
    {
        if (myUI != null)
            myUI.SetHighlight(myUI.Data == currentPlayer);

        foreach (var ui in enemySlots)
        {
            ui.SetHighlight(ui.Data == currentPlayer);
        }
    }
    public void ShowTrickUI(PlayerData player, List<CardInstance> valid, Action<CardInstance> callback)
    {
        StartCoroutine(TrickPopupRoutine(player, valid, callback));
    }
    // 계략 발동할지 선택 팝업
    IEnumerator TrickPopupRoutine(PlayerData player, List<CardInstance> valid, Action<CardInstance> callback)
    {
        Debug.Log($"{player.playerName} 계략 선택 대기");

        yield return new WaitForSeconds(1f);

        if (valid.Count > 0)
        {
            callback(valid[0]);
        }
        else
        {
            callback(null);
        }
    }

public void ShowCounterUI(PlayerData player, List<CardInstance> valid, Action<CardInstance> callback)
{
    StartCoroutine(CounterPopupRoutine(player, valid, callback));
}
// 체인?
IEnumerator CounterPopupRoutine(PlayerData player, List<CardInstance> valid, Action<CardInstance> callback)
{
    Debug.Log($"{player.playerName} Counter 선택 대기");

    yield return new WaitForSeconds(1f);

    if (valid.Count > 0)
    {
        callback(valid[0]);
    }
    else
    {
        callback(null);
    }
}
}
