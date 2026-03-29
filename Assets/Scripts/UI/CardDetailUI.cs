using UnityEngine;
using UnityEngine.UI;

public class CardDetailUI : MonoBehaviour
{
    public static CardDetailUI Instance;
    [SerializeField] private GameObject root;
    [SerializeField] private Image largeImage;
    public bool IsActive => root.activeSelf;
    private void Awake()
    {
        Instance = this;
        root.SetActive(false); 
    }
    public void Show(Sprite sprite)
    {
        if (sprite == null) return;

        largeImage.sprite = sprite;
        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }
}
