using UnityEngine;
using System; // Flags 네임스페이스
using System.Collections.Generic;

[Flags]
public enum CardCategory
{
    None = 0,
    Stack = 1 << 0, // 1
    Use = 1 << 1, // 2
    Keep = 1 << 2, // 4
    Trick = 1 << 3 // 8
}

[Flags]
public enum StackEffectType
{
    None = 0,
    Offensive = 1 << 0, // 스택 시 발동
    Defensive = 1 << 1 // 피격 시 발동
}

public enum CardBackType
{
    Normal,
    Arrow
}

[CreateAssetMenu(fileName = "Card", menuName = "Card/Card")]
public class Card : ScriptableObject
{
    [Header("기본 정보")]
    public string cardName;
    [TextArea] public string description;

    [Header("카드 분류")]
    public CardCategory category;

    [Header("이미지")]
    public CardBackType backType;
    public Sprite frontSprite;
    public Sprite backSprite;

    [Header("스택 카드 전용")]
    public int basePower;
    public StackEffectType stackEffectType = StackEffectType.None;

    [Header("효과 목록")]
    public List<CardEffect> effects;

    public bool HasCategory(CardCategory type)
    {
        return (category & type) != 0;
    }
}