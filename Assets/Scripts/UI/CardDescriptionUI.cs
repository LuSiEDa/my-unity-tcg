using UnityEngine;
using TMPro;

public class CardDescriptionUI : MonoBehaviour
{
    public static CardDescriptionUI Instance;
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        Instance = this;
        root.SetActive(false);
    }
    public void Show(string description)
    {
        text.text = description;
        root.SetActive(true);
    }
    public void Hide()
    {
        root.SetActive(false);
    }
}
