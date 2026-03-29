public enum CardZone
{
    Deck,
    Hand,
    Keep,
    Trick,
    Passive,
    Stack,
    LowStack,
    Use,
    Discard
}
public enum GameState
    {
        Idle,
        ResolvingAction,
        WaitingForTrick,
        ResolvingTrick,
        WaitingForCounter,
    }

public enum ActionType
{
    UseCard,
    StackAttack,
    PlaceKeep,
    PlaceTrick,
    OpenTrick,
    DrawCard,
    OpenPassive
}

public enum TurnPhase
{
    None,
    Start,
    Action,
    End,
    Transition
}

public enum DamageSourceType
{
    Stack,
    Effect,
    Lost
}

public enum TargetType
{
    None,
    Self,
    Opponent, // 그냥 상대방. (시스템이 정함)
    AllPlayers,
    AllOpponents,
    RandomPlayer,
    RandomOpponent,
    ChoosePlayer, // 선택
    LeftPlayer,
    RightPlayer,
    AdjacentPlayers // 양 옆
}

public enum LogType
{
    Info,
    Draw,
    Stack,
    Use,
    Trick,
    Keep,
    Damage,
    Chat
}

public enum DrawReason
{
    Normal,
    StackStart,
    StackHit,
    Effect
}

public enum CardTiming
{
    Anytime, // 언제든 사용
    StackOnly, // 스택 있을 때
    LowStack, // 로우스택 대응
    DamageIncoming, // 데미지 받을 때
    TurnStart, // 턴 시작
    TurnEnd // 턴 종료
}