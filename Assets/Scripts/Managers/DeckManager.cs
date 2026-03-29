using UnityEngine;
using System.Collections.Generic;
public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance;
    public List<Card> mainDeckData = new List<Card>(); // 설계도 목록
    private List<CardInstance> mainDeck = new List<CardInstance>(); // 실제 덱
    public List<CardInstance> MainDeck => mainDeck;

    public List<Card> arrowDeckData = new List<Card>();
    private List<CardInstance> arrowDeck = new List<CardInstance>();
    public List<CardInstance> ArrowDeck => arrowDeck;

    public System.Action OnDeckChanged;
    public System.Action OnArrowDeckChanged;

    private void Awake()
    {
        Instance = this;
        InitializeDeck();
    }
    // 초기화. 카드 객체생성하여 메인 덱 리스트 채우기
    void InitializeDeck()
    {
        mainDeck.Clear();
        foreach (Card card in mainDeckData)
        {
            mainDeck.Add(new CardInstance(card, null));
        }
        ShuffleMainDeck();
        OnDeckChanged?.Invoke();
    }
    // 덱 셔플
    public void ShuffleMainDeck()
    {
        if (mainDeck.Count <= 1) return;
        for (int i = 0; i <mainDeck.Count; i++)
        {
            CardInstance temp = mainDeck[i];
            int randomIndex =Random.Range(i, mainDeck.Count);
            mainDeck[i] = mainDeck[randomIndex];
            mainDeck[randomIndex] = temp;
        }
    }
    // 맨 위 카드 미리보기 mainDeck[0]에 있는 카드가 무엇인지
    public CardInstance PeekTop(int count)
    {
        if (mainDeck.Count == 0 || count < 1) return null;
        if (count > mainDeck.Count)
        {
            Debug.LogWarning($"{count}번째 카드를 보려 했지만, 덱에 {mainDeck.Count}장뿐입니다!");
            return null;
        }
        return mainDeck[count - 1];
    }
    // 드로우 
    public CardInstance Draw(PlayerData owner)
    {
        if (mainDeck.Count == 0)
        {
            Debug.Log("덱이 비었습니다!");
            // 향후 재셔플 로직
            return null;
        }
        CardInstance top = mainDeck[0];
        mainDeck.RemoveAt(0);

        EventBus.Publish(new DeckChangedEvent{ currentCount = mainDeck.Count});

        top.user = owner;
        return top;
    }
    // 화살 덱에서 메인덱으로 이동
    public void AddFromArrowToMain(CardInstance card)
    {
        arrowDeck.Remove(card);
        mainDeck.Add(card);
        ShuffleMainDeck();

        // 메인 덱 숫자가 늘어났음을 알려야 함!
        EventBus.Publish(new DeckChangedEvent { currentCount = mainDeck.Count });
        
        OnArrowDeckChanged?.Invoke();
    }
}
