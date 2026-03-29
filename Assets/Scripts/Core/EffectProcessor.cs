using UnityEngine;

public static class EffectProcessor
{
    public static void ExecuteEffects(CardInstance instance, PlayerData target)
    {
        if (instance?.origin?.effects == null) return;

        foreach (var effect in instance.origin.effects)
        {
            if (effect == null) continue;

            try
            {
                effect.Execute(instance.user, target, instance);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }

    public static int CalculateFinalPower(CardInstance instance)
    {
        if (instance?.origin == null)
            return 0;

        return instance.origin.basePower;
    }
}