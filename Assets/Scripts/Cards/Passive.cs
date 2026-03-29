using UnityEngine;

[CreateAssetMenu(fileName = "Passive", menuName = "Card/Passive")]
public class Passive : ScriptableObject
{
    public string passiveName;
    [TextArea] public string description;

    // 향후 이렇게 처리
    // public virtual void OnGameStart(playerData ownedr){}
    // public virtual void OnTurnStart(playerData ownedr){}
    // public virtual void OnTakeDamage(playerData ownedr, int amount){}
    //     PlayerData에 패시브 보유
    // public List<PassiveData> passives = new List<PassiveData>();
}
