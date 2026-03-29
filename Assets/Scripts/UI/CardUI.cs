using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI References")]
    public Image cardImage;
    [Header("Action Buttons")]
    public CardActionUI actionUI;
    [Header("Settings")]
    private CardInstance instance;
    public CardZone CurrentZone => instance.currentZone;

    public enum CardSize { S, M, L }

    private void Awake()
    {
        if (cardImage == null) cardImage = GetComponentInChildren<Image>();
    }

    /// <summary>
    /// HandUI에서 인스턴스를 전달받아 카드를 초기화합니다.
    /// </summary>
    public void Setup(CardInstance instance, CardSize size = CardSize.M)
    {
        this.instance = instance;

        UpdateVisual(size);
    }

    public void UpdateVisual(CardSize size)
    {
        if (instance == null || instance.origin == null || cardImage == null) return;

        cardImage.sprite = instance.isFaceUp
            ? instance.origin.frontSprite
            : instance.origin.backSprite;
        cardImage.rectTransform.localScale = Vector3.one;

        ApplySize(size);
    }

    private void ApplySize(CardSize size)
    {
        RectTransform rt = cardImage.rectTransform;
        switch (size)
        {
            case CardSize.S: rt.sizeDelta = new Vector2(80, 120); break;
            case CardSize.M: rt.sizeDelta = new Vector2(150, 225); break;
            case CardSize.L: rt.sizeDelta = new Vector2(500, 700); break;
        }
    }

    // --- [마우스 오버 효과: 쑉 올라오기] ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (instance == null) return;

        if (instance.currentZone == CardZone.Hand)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                transform.SetAsLastSibling(); // 맨 앞으로
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 80f);
            }
        }

        if (actionUI != null)
        {
            if (instance.currentZone == CardZone.Hand)
            {
                actionUI.gameObject.SetActive(true);
                // UI에 연결
                actionUI.Bind(instance);
            }
            else if (instance.currentZone == CardZone.Trick &&
                    !instance.isFaceUp &&
                    instance.user == TurnManager.Instance.localPlayer)
            {
                actionUI.gameObject.SetActive(true);
                actionUI.Bind(instance);
            }
            else
            {
                actionUI.Hide();
            }
        }
        CardDescriptionUI.Instance.Show(instance.origin.description);
        return;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (instance == null) return;

        if (instance.currentZone == CardZone.Hand)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f);
            }
        }

        if (actionUI != null)
        {
            actionUI.gameObject.SetActive(false);
        }
        CardDescriptionUI.Instance.Hide();
        return;
    }

    // --- [클릭 효과: 메뉴 띄우기] ---
    public void OnPointerClick(PointerEventData eventData)
    {
        if (CardDetailUI.Instance != null && CardDetailUI.Instance.IsActive)
        {
            CardDetailUI.Instance.Hide();
            return;
        }
        if (instance == null) return;
        if (instance.currentZone == CardZone.Hand) return;

        bool isMine = instance.user == TurnManager.Instance.localPlayer;
        bool isFaceUp = instance.isFaceUp;

        if (!isFaceUp && !isMine) return;

        ShowCardDetail();
    }

    private void ShowCardDetail()
    {
        if (instance == null) return;
        if (CardDetailUI.Instance == null) return;

        if (CardDetailUI.Instance.IsActive)
        {
            CardDetailUI.Instance.Hide();
            return;
        }

        Sprite sprite = GetPopupSprite();
        if (sprite == null) return;

        CardDetailUI.Instance.Show(sprite);
    }

    private void OnClick()
    {
        if (instance == null) return;

        // [이벤트 전달] StackManager에게 이 카드를 냈다고 알림
        StackManager.Instance.PlayStackCard(instance);
    }

    public void Flip(bool faceUp, CardSize size)
    {
        instance.isFaceUp = faceUp;
        UpdateVisual(size);
    }

    public Sprite GetPopupSprite() => instance?.origin?.frontSprite;
}