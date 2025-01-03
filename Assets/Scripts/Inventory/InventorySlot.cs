using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSO;
    [SerializeField] private bool isUnlocked = false;

    public WeaponSO GetWeaponInfo()
    {
        return isUnlocked ? weaponSO : null; //nomes retorno l'arma si el slot esta desbloquejat
    }

    public void UnlockSlot() //no es una tenda com a tal, pero necessites matar a x enemics per a desbloquejar-la
    {
        isUnlocked = true;
    }
}
