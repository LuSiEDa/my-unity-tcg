using UnityEngine;
using System;
using System.Collections.Generic;
using IEnumerator = System.Collections.IEnumerator; // 별명 지정!

public class ActionQueue : MonoBehaviour
{
    public static ActionQueue Instance;
    private static Queue<IEnumerator> queue = new Queue<IEnumerator>();
    private static bool isProcessing = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
            Destroy(gameObject);
    }

    public static void Enqueue(IEnumerator action)
    {
        queue.Enqueue(action);

        if (!isProcessing && Instance != null)
        {
            Instance.StartCoroutine(ProcessQueue());
        }
    }
    private static IEnumerator ProcessQueue()
    {
        isProcessing = true;

        while (queue.Count > 0)
        {
            // Dequeue() : Queue<T> 자료구조가 이미 가지고 있는 메서드임. 
            yield return Instance.StartCoroutine(queue.Dequeue());
        }
        isProcessing = false;
    }
}
