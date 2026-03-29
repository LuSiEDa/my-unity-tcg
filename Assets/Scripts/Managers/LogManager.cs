using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogManager : MonoBehaviour
{
    public static LogManager Instance;
    [SerializeField] private Transform logContent;
    [SerializeField] private GameObject logTextPrefab;
    [SerializeField] ScrollRect scroll;
    [SerializeField] int maxLogs = 50;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void AddLog(string message, LogType type = LogType.Info)
    {
        if (logContent == null || logTextPrefab == null) return;

        bool shouldAutoScroll = scroll.verticalNormalizedPosition <= 0.01f;

        GameObject go = Instantiate(logTextPrefab, logContent);
        TMP_Text text = go.GetComponent<TMP_Text>();

        text.text = message;
        text.color = GetColor(type);
        if (logContent.childCount > maxLogs)
        {
            Destroy(logContent.GetChild(0).gameObject);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)logContent);
        if (shouldAutoScroll)
        {
            scroll.StopMovement();
            scroll.verticalNormalizedPosition = 0f;
        }
    }
    Color GetColor(LogType type)
    {    
        switch (type)
        {
            case LogType.Draw: return new Color32(0, 255, 120, 255);
            case LogType.Stack: return new Color32(255, 220, 0, 255);
            case LogType.Use: return new Color32(0, 200, 255, 255);
            case LogType.Trick: return new Color32(255, 160, 0, 255);
            case LogType.Keep: return new Color32(120, 255, 120, 255);
            case LogType.Damage: return new Color32(255, 70, 70, 255);
            case LogType.Chat: return new Color32(120, 255, 255, 255);
        }
        return Color.white;
    }
}
