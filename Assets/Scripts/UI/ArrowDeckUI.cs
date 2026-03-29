using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArrowDeckUI : MonoBehaviour
{
    [Header("References")]
    public Image deckImage;
    public TextMeshProUGUI countText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeckManager.Instance.OnArrowDeckChanged += Refresh;
        Refresh();
    }
    public void Refresh()
    {
        int count = DeckManager.Instance.ArrowDeck.Count;

        if (count <= 0)
        {
            deckImage.enabled =false;
            countText.text = "0";
            return;
        }
        deckImage.enabled = true;
        countText.text = count.ToString();
    }
}
