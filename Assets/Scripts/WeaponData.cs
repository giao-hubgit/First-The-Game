using UnityEditor.AdaptivePerformance.Editor;
using UnityEngine;

public enum WeaponType
{
    Ranged,
    Melee
}

public class WeaponData : ScriptableObject
{
    public string weaponName;
    public WeaponType weaponType;
    public GameObject weaponPrefab;
    public Color outlineColor = Color.white;
    public Sprite weaponIcon;
}