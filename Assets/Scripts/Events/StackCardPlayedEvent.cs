using UnityEngine;

public class StackCardPlayedEvent
{
    public CardInstance instance;
    public StackCardPlayedEvent(CardInstance instance)
    {
        this.instance = instance;
    }
}
