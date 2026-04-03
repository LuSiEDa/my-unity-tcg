using UnityEngine;

public class EffectContext
{
    // 방금 파괴된 카드
    public CardInstance lastCard;
    // 읽어온 파괴력 계산값
    public int lastValue;
    // 이 Context는 Effect 실행 중에만 사용되는 임시 데이터다.

    // 상태 플래그
    public bool isImmediate;
    public bool isDamageIncoming;
    public bool isLowStack;
    public bool isDamageResolved;
    public bool isTurnStart;
    public bool isTurnEnd;
    public bool isStackPlayed;
    // 실행 정보
    public PlayerData source;
    public PlayerData target;
}
