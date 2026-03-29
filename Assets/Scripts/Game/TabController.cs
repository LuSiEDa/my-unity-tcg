using UnityEngine;

public class TabController : MonoBehaviour
{
    [SerializeField] GameObject logPanel;
    [SerializeField] GameObject chatPanel;

    public void ShowLog()
    {
        logPanel.SetActive(true);
        chatPanel.SetActive(false);
    }
    public void ShowChat()
    {
        logPanel.SetActive(false);
        chatPanel.SetActive(true);
    }
}
