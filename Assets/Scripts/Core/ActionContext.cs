using UnityEngine;

public class ActionContext
{
    // AP를 소모할지 말지.
    // true면 포인트 깎음, false면 소모없이 공짜로 실행
    public bool consumeActionPoint;
    public DrawReason drawReason;
    public ActionContext(bool consumeActionPoint = true, DrawReason reason = DrawReason.Normal)
    {
        this.consumeActionPoint = consumeActionPoint;
        drawReason = reason;
    }
}
