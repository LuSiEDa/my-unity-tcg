using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckUI : MonoBehaviour
{
    public Image deckImage;
    public TextMeshProUGUI countText;

    private void Start()
    {
        UpdateTopCardVisual();
    }
    private void OnEnable()
    {
        // 장부에 이름 올리기
        EventBus.Subscribe<DeckChangedEvent>(OnDeckChanged);
    }

    private void OnDisable()
    {
        // 씬이 바뀌거나 객체가 사라질 때 장부에서 이름 빼기 (메모리 누수 방지)
        EventBus.Unsubscribe<DeckChangedEvent>(OnDeckChanged);
    }

    // 이벤트가 터졌을 때 실행될 함수
    private void OnDeckChanged(DeckChangedEvent evt)
    {
        countText.text = evt.currentCount.ToString();
        UpdateTopCardVisual(); // 기존 비주얼 업데이트 로직 호출
    }
    public void UpdateTopCardVisual()
    {
        UpdateTopCardVisual(1); // 기본 1장만 표시
    }
    public void UpdateTopCardVisual(int count)
    {
        countText.text = DeckManager.Instance.MainDeck.Count.ToString();

        CardInstance top = DeckManager.Instance.PeekTop(count);
        if (top == null)
        {
            deckImage.enabled = false;
            return;
        }

        deckImage.enabled = true;
        deckImage.sprite = top.origin.backSprite;
    }

    public void OnClickDraw()
    {
        PlayerData player = TurnManager.Instance.CurrentPlayer;
        if (player != null && player.actionPoint > 0)
            TurnManager.Instance.ExecutePlayerAction(player, ActionType.DrawCard);
    }
}