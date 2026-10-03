using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Dungeon/Buff Asset")]
public class BuffSO : ScriptableObject
{
    public string buffName;
    public Sprite icon;
    [TextArea] public string description;

    // Logic cộng chỉ số tùy biến
    public virtual void ApplyBuff(GameObject player)
    {
        // VD: PlayerStats.Instance.AddMaxHP(20);
    }
}