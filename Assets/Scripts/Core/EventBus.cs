using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static Dictionary<Type, List<Delegate>> events = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        var type = typeof(T); // 어떤 종류(T)의 이벤트인지 타입을 알아냄. (예: DamageEvent)
        // 만약 장부에 이 이벤트 종류가 등록된 적이 없다면?
        if (!events.ContainsKey(type))
            events[type] = new List<Delegate>(); // 빈 목록을 하나 새로 만듬.

        // (중요) 장부에 "이 소식이 오면 이 함수(callback)를 실행해줘!"라고 추가.
        events[type].Add(callback);
    }
    // 구독 취소하기
    public static void Unsubscribe<T>(Action<T> callback)
    {
        var type = typeof(T);
        if (events.ContainsKey(type))
        {
            // 장부에서 해당 함수를 빼버림.
            // (주의: 람다식 사용 시 정확한 참조 비교가 필요하여 실제 구현은 보통 List를 씁니다)
            events[type].Remove(callback);
        }
    }
    public static void Publish<T>(T evt)
    {
        var type = typeof(T); // 지금 터진 사건(evt)이 어떤 종류인지 확인
        // 장부에 이 소식을 듣고 싶어 하는 사람들이 있는지 확인
        if (!events.ContainsKey(type)) return;

        // (핵심) 등록된 모든 사람(함수)들에게 "자, 소식 왔다!"라며 이벤트를 전달(Invoke).
        foreach (var del in events[type])
        {
            var callback = (Action<T>)del;
            ActionQueue.Enqueue(InvokeEvent(callback, evt));
        }
    }

    private static IEnumerator InvokeEvent<T>(Action<T> callback, T evt)
    {
        callback?.Invoke(evt);
        yield return null;
    }
}