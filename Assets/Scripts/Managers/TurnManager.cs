using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public List<PlayerData> players { get; private set; }
    public PlayerData localPlayer { get; private set; }
    public int currentTurnIndex = 0;
    public bool isClockwise = true;
    private TurnPhase currentPhase = TurnPhase.None;
    // 카드 타이밍 검사에서 사용할 프로퍼티
    public TurnPhase CurrentPhase => currentPhase;

    public PlayerData CurrentPlayer => players[currentTurnIndex];
    private bool initialized = false;
    // 스택 대응 여부
    private bool respondedToStack = false;
    // 엑스트라 턴 체크
    private bool extraTurn = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnEnable()
    {
        EventBus.Subscribe<ActionPointChangedEvent>(OnAPChanged);
        EventBus.Subscribe<LowStackResolvedEvent>(OnLowStackResolved);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ActionPointChangedEvent>(OnAPChanged);
        EventBus.Unsubscribe<LowStackResolvedEvent>(OnLowStackResolved);
    }

    private void OnAPChanged(ActionPointChangedEvent e)
    {
        if (e.player == CurrentPlayer && e.currentAP <= 0)
        {
            EndTurn();
        }
    }

    public void Initialize(List<PlayerData> createdPlayers, PlayerData local)
    {
        if (initialized) return;
        players = createdPlayers;
        localPlayer = local;
        currentTurnIndex = Random.Range(0, players.Count);
        initialized = true;
    }

    public void StartGame()
    {
        EventBus.Publish(new GameStartEvent(players, localPlayer));
        int startingHandSize = players.Count;
    
        foreach (var player in players) // 각 플레이어 반복
        {
            for (int i = 0; i < startingHandSize; i++)
            {
                GameActionController.Instance.Execute(
                    player, 
                    ActionType.DrawCard,
                    new ActionContext(false));
            }
        }

        StartTurn();
    }

    public void StartTurn()
    {
        ActionQueue.Enqueue(StartTurnSequence());
    }
    private IEnumerator StartTurnSequence()
    {
        currentPhase = TurnPhase.Start;
        PlayerData player = CurrentPlayer;
        respondedToStack = false;
        if (extraTurn)
            player.actionPoint += 1;
        else 
            player.actionPoint = 1;
        extraTurn = false;
        // 현재 플레이어 변경 이벤트
        EventBus.Publish(new CurrentPlayerChangedEvent(player));
        // 턴 시작 이벤트 발행
        EventBus.Publish(new TurnStartEvent(player));
        yield return null;

        currentPhase = TurnPhase.Action;

        if (player != localPlayer)
            ActionQueue.Enqueue(CPUPlay(player));
    }

    public void ExecutePlayerAction(PlayerData user, ActionType action, CardInstance instance = null, PlayerData target = null)
    {
        if (!CanPlayerAct(user)) return;

        ActionQueue.Enqueue(ExecuteWithStackCheck(user, action, instance, target));
    }
    private IEnumerator ExecuteWithStackCheck(PlayerData user, ActionType action, CardInstance instance, PlayerData target)
    {
        // 스택 열려있으면 먼저 해결
        if (StackManager.Instance.IsStackOpen && action != ActionType.StackAttack)
        {
            yield return StackManager.Instance.ResolveNormalStack();
        }
        if (action == ActionType.StackAttack)
        {
            respondedToStack = true;
        }

        // 지속 카드 대상 선택
        if (action == ActionType.PlaceKeep && instance != null && target == null)
        {
            StartSelectingKeepTarget(user, instance);
            yield break;
        }
        GameActionController.Instance.Execute(
            user,
            action,
            new ActionContext(true),
            instance,
            target
        );
    }

    private void StartSelectingKeepTarget(PlayerData user, CardInstance instance)
    {
        GameInputManager.Instance.StartSelectingPlayer((selectedPlayer) =>
        {
            GameActionController.Instance.Execute(
                user, 
                ActionType.PlaceKeep, 
                new ActionContext(true), 
                instance, 
                selectedPlayer);

            EventBus.Publish(new UIMessageHideEvent());
        });
    }

    private bool CanPlayerAct(PlayerData user)
    {
        return user == CurrentPlayer && currentPhase == TurnPhase.Action && user.actionPoint > 0;
    }

    public void EndTurn()
    {
        ActionQueue.Enqueue(EndTurnSequence());
    }
    private IEnumerator EndTurnSequence()
    {
        currentPhase = TurnPhase.End;
        PlayerData player = CurrentPlayer;

        EventBus.Publish(new TurnEndedEvent(player));
        yield return null;
        
        // 스택 평가 (턴 종료 시 실행)
        if (StackManager.Instance.IsStackOpen && !respondedToStack)
        {
            yield return StackManager.Instance.ResolveNormalStack();
        }

        currentPhase = TurnPhase.Transition;
        if (!extraTurn)
        {
            AdvanceTurnIndex();
        }

        yield return null;

        StartTurn();
    }

    private void AdvanceTurnIndex()
    {
        currentTurnIndex = isClockwise
            ? (currentTurnIndex + 1) % players.Count
            : (currentTurnIndex - 1 + players.Count) % players.Count;
    }

    public void GrantExtraTurn(PlayerData user)
    {
        if (user != CurrentPlayer) return;
        extraTurn = true;
    }
    private void OnLowStackResolved(LowStackResolvedEvent e)
    {
        if (e.Target == CurrentPlayer)
        {
            extraTurn = true;
            EventBus.Publish(new ExtraTurnGrantedEvent
            {
                player = CurrentPlayer
            });
        }
    }


    // 임시 CPU플레이어 코드. 나중에 제거 또는 분리 요망
    private IEnumerator CPUPlay(PlayerData player)
    {
        yield return new WaitForSeconds(1f);

        if (!CanPlayerAct(player))
            yield break;

        CardInstance chosen = null;

        foreach (var card in player.hand)
        {
            if (StackManager.Instance.IsStackOpen)
            {
                chosen = card;
                break;
            }
            else
            {
                if (card.BasePower > 0)
                {
                    chosen = card;
                    break;
                }
            }
        }

        if (chosen != null)
        {
            ExecutePlayerAction(player, ActionType.StackAttack, chosen);
        }
    }
}