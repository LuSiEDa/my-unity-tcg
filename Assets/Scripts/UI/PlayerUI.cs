using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class PlayerUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Common UI")]
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI lifeText;
    [SerializeField] protected TextMeshProUGUI actionText;

    [SerializeField] private Image selectHighlightImage;
    private Image bgImage;
    private PlayerData data;
    public PlayerData Data => data;
    private bool isSelectable = false;
    [SerializeField] private Transform handZone;
    [SerializeField] private Transform keepZone;
    [SerializeField] private Transform trickZone;
    [SerializeField] private Transform passiveZone;

    // 컴퍼넌트 캐싱
    protected virtual void Awake()
    {
        bgImage = GetComponent<Image>();
        SetHighlight(false);
    }
    public virtual void Setup(PlayerData playerData)
    {
        data = playerData;
        RegisterEvents();
        Refresh();
    }
    protected virtual void RegisterEvents()
    {
        // 낡은 UIUpdateEvent는 제거하거나 유지 (하단에서 개별 이벤트 추가)
        EventBus.Subscribe<CardDrawnEvent>(OnCardDrawn);
        EventBus.Subscribe<ActionPointChangedEvent>(OnActionPointChanged);
        EventBus.Subscribe<CurrentPlayerChangedEvent>(OnTurnChanged);
    }
    private void OnEnable()
    {
        EventBus.Subscribe<UIUpdateEvent>(OnUIUpdate);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<UIUpdateEvent>(OnUIUpdate);
    }
    private void OnUIUpdate(UIUpdateEvent evt)
    {
        Refresh();
    }
protected virtual void OnCardDrawn(CardDrawnEvent evt)
    {
        if (evt.player == data) Refresh();
    }

    protected virtual void OnActionPointChanged(ActionPointChangedEvent evt)
    {
        if (evt.player == data) Refresh();
    }

    protected virtual void OnTurnChanged(CurrentPlayerChangedEvent evt)
    {
        // 내 턴이면 하이라이트 켜기
        SetHighlight(evt.currentPlayer == data);
    }

    // 화면 갱신
    public virtual void Refresh()
    {
        if (data == null) return;
        if (nameText != null) 
            nameText.text = data.playerName;
        if (lifeText != null) 
            lifeText.text = $"Life : {data.life}";
        if (actionText != null) 
            actionText.text = $"AP : {data.actionPoint}";
    }
    public void SetSelectable(bool value)
    {
        isSelectable = value;
        SetSelectHighlight(value);
    }

    // 플레이어 UI를 클릭하면 게임 인풋매니저에 전달된다.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isSelectable)
            return;
        GameInputManager.Instance.TrySelectPlayer(Data);
    }

    // 시각적 피드백(하이라이트)
    public virtual void SetHighlight(bool value)
    {
        if (bgImage != null)
        bgImage.color = value ? Color.yellow : Color.gray;
    }
    // 선택 가능 표시 (별도 레이어)
    protected virtual void SetSelectHighlight(bool value)
    {
        if (selectHighlightImage != null)
            selectHighlightImage.gameObject.SetActive(value);
    }
    public virtual Transform GetZone(CardZone zone)
    {
        switch (zone)
        {
            case CardZone.Hand: return handZone;
            case CardZone.Keep: return keepZone;
            case CardZone.Trick: return trickZone;
            case CardZone.Passive: return passiveZone;
        }
        return null;
    }
}
