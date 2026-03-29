using UnityEngine;

public class LogListener : MonoBehaviour
{
        private void OnEnable()
    {
        EventBus.Subscribe<TurnStartEvent>(OnTurnStart);
        EventBus.Subscribe<CardDrawnEvent>(OnDraw);
        EventBus.Subscribe<DamageResolvedEvent>(OnDamage);
        EventBus.Subscribe<LowStackResolvedEvent>(OnLowStack);
        EventBus.Subscribe<CardUsedEvent>(OnUse);
        EventBus.Subscribe<TrickPlacedEvent>(OnTrickPlaced);
        EventBus.Subscribe<TrickOpenedEvent>(OnTrickOpened);
        EventBus.Subscribe<PlaceKeepEvent>(OnKeep);
        EventBus.Subscribe<ExtraTurnGrantedEvent>(OnExtraTurn);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<TurnStartEvent>(OnTurnStart);
        EventBus.Unsubscribe<CardDrawnEvent>(OnDraw);
        EventBus.Unsubscribe<DamageResolvedEvent>(OnDamage);
        EventBus.Unsubscribe<LowStackResolvedEvent>(OnLowStack);
        EventBus.Unsubscribe<CardUsedEvent>(OnUse);
        EventBus.Unsubscribe<TrickPlacedEvent>(OnTrickPlaced);
        EventBus.Unsubscribe<TrickOpenedEvent>(OnTrickOpened);
        EventBus.Unsubscribe<PlaceKeepEvent>(OnKeep);
        EventBus.Unsubscribe<ExtraTurnGrantedEvent>(OnExtraTurn);
    }

    void OnTurnStart(TurnStartEvent e)
    {
        LogManager.Instance.AddLog(
            $"▶ {e.Player.playerName} 턴 시작",
            LogType.Info
        );
    }

    void OnDraw(CardDrawnEvent e)
    {
        string msg = "";
        switch (e.reason)
        {
            case DrawReason.Normal:
                msg = $"{e.player.playerName} 이(가) 1장 드로우";
                break;
            case DrawReason.StackStart:
                msg = $"{e.player.playerName} 최초의 스택 드로우";
                break;
            case DrawReason.StackHit:
                msg = $"{e.player.playerName} 이(가) 스택 드로우";
                break;
            case DrawReason.Effect:
                msg = $"{e.player.playerName} 이(가) 효과로 드로우";
                break;
        }
        LogManager.Instance.AddLog(msg, LogType.Draw);
    }

    void OnDamage(DamageResolvedEvent e)
    {
        LogManager.Instance.AddLog(
            $"{e.Target.playerName} {e.FinalDamage} 데미지",
            LogType.Damage
        );
    }

    void OnLowStack(LowStackResolvedEvent e)
    {
        LogManager.Instance.AddLog(
            $"{e.ResolvedCard.user.playerName} 이(가) {e.ResolvedCard.origin.cardName} 로우 스택!",
            LogType.Stack
        );
    }
    void OnUse(CardUsedEvent e)
    {
        LogManager.Instance.AddLog(
            $"{e.user.playerName} 이(가) {e.instance.origin.cardName} 사용",
            LogType.Use
        );
    }

    private void OnTrickPlaced(TrickPlacedEvent e)
    {
        LogManager.Instance.AddLog($"{e.user.playerName} 이(가) 계략 설치",
        LogType.Trick
        );
    }

    private void OnTrickOpened(TrickOpenedEvent e)
    {
        LogManager.Instance.AddLog($"{e.user.playerName}의 계략 {e.instance.origin.cardName} 오픈",
        LogType.Trick
        );
    }

    void OnKeep(PlaceKeepEvent e)
    {
        LogManager.Instance.AddLog(
            $"{e.user.playerName} 이(가) {e.target.playerName} 에게 지속 {e.instance.origin.cardName} 설치",
            LogType.Keep
        );
    }
    void OnExtraTurn(ExtraTurnGrantedEvent e)
    {
        LogManager.Instance.AddLog(
            $"★ {e.player.playerName} 엑스트라 턴!",
            LogType.Info
        );
    }
}
