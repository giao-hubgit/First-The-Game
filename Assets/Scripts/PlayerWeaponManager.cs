using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    private PlayerRangedWeapon rangedController;
    private PlayerMeleeWeapon meleeController;

    void Awake()
    {
        rangedController = GetComponent<PlayerRangedWeapon>();
        meleeController = GetComponent<PlayerMeleeWeapon>();
    }

    public bool IsSlotEmpty(WeaponType type)
    {
        if (type == WeaponType.Ranged)
        {
            return rangedController.currentWeapon == null || rangedController.currentWeapon == rangedController.nullWeapon;
        }
        else if (type == WeaponType.Melee)
        {
            return meleeController.currentWeapon == null;
        }
        return false;
    }

    public void EquipWeapon(WeaponData newWeapon)
    {
        if (newWeapon.weaponType == WeaponType.Ranged)
        {
            WeaponRangedData rangedData = newWeapon as WeaponRangedData;
            if (rangedData != null)
            {
                rangedController.Equip(rangedData);
                Debug.Log($"Đã trang bị súng: {rangedData.weaponName}");
            }
        }
        else if (newWeapon.weaponType == WeaponType.Melee)
        {
            WeaponMeleeData meleeData = newWeapon as WeaponMeleeData;
            if (meleeData != null)
            {
                meleeController.Equip(meleeData);
                Debug.Log($"Đã trang bị cận chiến: {meleeData.weaponName}");
            }
        }
    }
}