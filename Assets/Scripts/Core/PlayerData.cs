using System.Collections.Generic;

public class PlayerData
{
    // 기본 상태
    public string playerName;
    public int life = 100;
    public int actionPoint = 0;
    public int index;
    public bool skipStackDraw = false;

    // 카드 상태
    public List<CardInstance> hand = new();
    public CardInstance activeKeepCard;
    public CardInstance trickCard;

    // 지속 효과 보관 (데이터만)
    private readonly List<CardEffect> activeEffects = new();
    public IReadOnlyList<CardEffect> ActiveEffects => activeEffects;

    public PlayerData(string name, int idx)
    {
        playerName = name;
        index = idx;
    }

    // ===== 상태 조작용 최소 함수 =====

    public void AddCardToHand(CardInstance instance)
    {
        if (instance == null) return;
        hand.Add(instance);
        instance.user = this;
    }

    public void RemoveCardFromHand(CardInstance instance)
    {
        if (instance == null) return;
        hand.Remove(instance);
    }

    public void ModifyLife(int amount)
    {
        life += amount;
        if (life < 0) life = 0;
    }

    public void ModifyActionPoint(int amount)
    {
        actionPoint += amount;
        if (actionPoint < 0) actionPoint = 0;
    }

    public void SetKeepCard(CardInstance instance)
    {
        activeKeepCard = instance;
        if (instance != null)
            instance.user = this;
    }

    public void SetTrickCard(CardInstance instance)
    {
        trickCard = instance;
        if (instance != null)
            instance.user = this;
    }
}