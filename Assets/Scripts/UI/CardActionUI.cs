using UnityEngine;
using UnityEngine.UI;

public class CardActionUI : MonoBehaviour
{
    public Button btnStack;
    public Button btnUse;
    public Button btnKeep;
    public Button btnTrick;
    private CardInstance currentInstance;

    private Color enabledColor = new Color32(255, 87, 34, 255);
    private Color disabledColor = new Color32(128,128,128,255);

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Bind(CardInstance instance)
    {
        currentInstance = instance;
        if (currentInstance == null)
            return;
        gameObject.SetActive(true);
        RefreshButtons();
    }

    public void RefreshButtons()
    {
        if (currentInstance == null) return;
        UpdateButton(btnStack, currentInstance.CanStack());
        UpdateButton(btnUse, currentInstance.CanUse());
        UpdateButton(btnKeep, currentInstance.CanKeep());
        bool trickActive = false;
        if (currentInstance.currentZone == CardZone.Hand && currentInstance.CanTrick())
            trickActive = true; // 패에서 설치 가능
        else if (currentInstance.currentZone == CardZone.Trick && !currentInstance.isFaceUp)
            trickActive = true; // 설치된 Trick 카드 오픈 가능

        UpdateButton(btnTrick, trickActive);

        // 클릭 이벤트 등록
        btnStack.onClick.RemoveAllListeners();
        btnStack.onClick.AddListener(() =>
        {
            TurnManager.Instance.ExecutePlayerAction(
                currentInstance.user,
                ActionType.StackAttack,
                currentInstance
            );
        });

        btnUse.onClick.RemoveAllListeners();
        btnUse.onClick.AddListener(() =>
        {
            TurnManager.Instance.ExecutePlayerAction(
                currentInstance.user,
                ActionType.UseCard,
                currentInstance
            );
        });

        btnKeep.onClick.RemoveAllListeners();
        btnKeep.onClick.AddListener(() =>
        {
            TurnManager.Instance.ExecutePlayerAction(
                currentInstance.user,
                ActionType.PlaceKeep,
                currentInstance
            );
        });

        btnTrick.onClick.RemoveAllListeners();
        btnTrick.onClick.AddListener(() =>
        {
            if (currentInstance.currentZone == CardZone.Hand)
            {
                TurnManager.Instance.ExecutePlayerAction(
                    currentInstance.user,
                    ActionType.PlaceTrick,
                    currentInstance
                );
            }
            else if (currentInstance.currentZone == CardZone.Trick && !currentInstance.isFaceUp)
            {
                TurnManager.Instance.ExecutePlayerAction(
                    currentInstance.user,
                    ActionType.OpenTrick,
                    currentInstance
                );
            }
        });
    }

    private void UpdateButton(Button button, bool isActive)
    {
        button.interactable = isActive;
        var colors = button.colors;
        colors.normalColor = isActive ? enabledColor : disabledColor;
        colors.highlightedColor = isActive ? enabledColor : disabledColor;
        colors.pressedColor = isActive ? enabledColor : disabledColor;
        button.colors = colors;
    }

    public void Hide()
    {
        currentInstance = null;
        gameObject.SetActive(false);
    }
}