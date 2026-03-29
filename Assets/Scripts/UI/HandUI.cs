using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandUI : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;
    private PlayerData player;

    private void OnEnable()
    {
        EventBus.Subscribe<UIUpdateEvent>(OnUIUpdate);
    }
    private void OnDisable()
    {
        EventBus.Unsubscribe<UIUpdateEvent>(OnUIUpdate);
    }
    private void OnUIUpdate(UIUpdateEvent evt)
    {
        Refresh();
    }

    // UI와 핸드 데이터 모델 바인딩
    public void Setup(PlayerData playerData)
    {
        player = playerData;
        Refresh();
    }
    // 핸드 그래픽 갱신 (현재 손패 상태를 화면에 동기화)
    public void Refresh()
    {
        if (player == null || cardPrefab == null) return;

        // 1. 기존 카드 싹 지우기 (중복 생성 방지)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // 2. 카드 생성
        foreach (var instance in player.hand)
        {
            // 부모를 지정하지 않고 일단 생성
            GameObject obj = Instantiate(cardPrefab,transform, false);

            RectTransform rt = obj.GetComponent<RectTransform>();

            if (rt != null)
            {
                rt.localScale = Vector3.one;
                rt.localRotation = Quaternion.identity;
            }
            CardUI cardUI = obj.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.Setup(instance);
            }
        }
        UpdateLayout();
    }
    // 핸드에서 카드 배치
    private void UpdateLayout()
    {
        int count = transform.childCount;
        if (count == 0) return;

        RectTransform area = GetComponent<RectTransform>();
        RectTransform firstCard = transform.GetChild(0).GetComponent<RectTransform>();

        float cardWidth = firstCard.rect.width;
        float areaWidth = area.rect.width;
        float maxSpacing = cardWidth * 0.9f;
        float spacing = (count > 1) ? (areaWidth - cardWidth) / (count - 1) : 0;

        if (spacing > maxSpacing)
        {
            spacing = maxSpacing;
        }
        float startX = -(count - 1) * spacing / 2f;

        for (int i = 0; i < count; i++)
        {
            RectTransform rt = transform.GetChild(i).GetComponent<RectTransform>();
            
            rt.localScale = Vector3.one;
            float x = startX + i * spacing;
            rt.anchoredPosition = new Vector2(x, 0f);
        }
    }

    private void OnDestroy()
    {
    }
}
