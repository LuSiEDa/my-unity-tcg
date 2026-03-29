using UnityEngine;

public class EffectContext
{
    // 방금 파괴된 카드
    public CardInstance lastCard;
    // 읽어온 파괴력 계산값
    public int lastValue;
    // 이 Context는 Effect 실행 중에만 사용되는 임시 데이터다.
}
