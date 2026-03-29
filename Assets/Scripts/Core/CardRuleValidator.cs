using UnityEngine;

public static class CardRuleValidator
{
    public static bool CanPlayCard(
        PlayerData user,
        CardInstance instance,
        ActionType action
    )
    {
        if (user == null || instance == null)
            return false;
        // 카드 타입 검사
        if (!ValidateCategory(instance, action))
            return false;
        
        return true;
    }
    private static bool ValidateCategory(CardInstance instance, ActionType action)
    {
        switch (action)
        {
            case ActionType.UseCard:
                return instance.CanUse();

            case ActionType.StackAttack:
                return instance.CanStack();

            case ActionType.PlaceKeep:
                return instance.CanKeep();

            case ActionType.PlaceTrick:
                return instance.CanTrick();
        }

        return true;
    }

    private static bool ValidateTiming(CardInstance instance)
    {
        if (instance.origin?.effects == null)
            return true;

        foreach (var effect in instance.origin.effects)
        {
            if (GameActionController.CheckTiming(effect.timing))
                return true;
        }

        return false;
    }
}
