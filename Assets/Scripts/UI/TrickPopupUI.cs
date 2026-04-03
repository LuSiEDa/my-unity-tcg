using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class TrickPopupUI : MonoBehaviour
{
    public static TrickPopupUI Instance;

    public GameObject panel;
    public TextMeshProUGUI descriptionText;
    public Button YesButton;
    public Button noButton;

    private PlayerData currentPlayer;
    private CardInstance currentTrick;
    
    // 응답하는 쪽
    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        EventBus.Subscribe<TrickDecisionEvent>(OnTrickDecision);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<TrickDecisionEvent>(OnTrickDecision);
    }
    void OnTrickDecision(TrickDecisionEvent e)
    {
        currentPlayer = e.player;
        currentTrick = e.instance;

        panel.SetActive(true);
        descriptionText.text = $"{currentTrick.origin.cardName} 발동?";
    }
    public void OnClickYes()
    {
        EventBus.Publish(new TrickSelectedEvent(currentPlayer, currentTrick));
        Close();
    }
    public void OnClickNo()
    {
        EventBus.Publish(new TrickSelectedEvent(currentPlayer, null));
        Close();
    }
    void Close()
    {
        panel.SetActive(false);
    }
}
