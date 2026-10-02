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

    public void EquipWeapon(WeaponData newWeapon, int ammo = -1)
    {
        if (newWeapon.weaponType == WeaponType.Ranged)
        {
            WeaponRangedData rangedData = newWeapon as WeaponRangedData;
            if (rangedData != null)
            {
                if (!IsSlotEmpty(WeaponType.Ranged))
                {
                    rangedController.DropCurrentWeapon();
                }

                rangedController.Equip(rangedData, ammo);
                Debug.Log($"Ranged weapon equipped: {rangedData.weaponName}");
            }
        }
        else if (newWeapon.weaponType == WeaponType.Melee)
        {
            WeaponMeleeData meleeData = newWeapon as WeaponMeleeData;
            if (meleeData != null)
            {
                if (!IsSlotEmpty(WeaponType.Melee))
                {
                    meleeController.DropCurrentWeapon();
                }

                meleeController.Equip(meleeData);
                Debug.Log($"Melee weapon equipped: {meleeData.weaponName}");
            }
        }
    }
}