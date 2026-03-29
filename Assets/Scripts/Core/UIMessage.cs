using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIMessage : MonoBehaviour
{
    public static UIMessage Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string msg)
    {
        panel.SetActive(true);
        text.text = msg;
    }
    public void Hide()
    {
        panel.SetActive(false);
    }
}
