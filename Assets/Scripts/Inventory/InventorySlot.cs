using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private WeaponSO weaponSO;
    [SerializeField] private bool isUnlocked = false;
    [SerializeField] private GameObject itemImage;

    private Image itemImageComponent;

    private void Awake()
    {
        itemImageComponent = itemImage.GetComponent<Image>();
        UpdateSlotTransparency();
    }

    public WeaponSO GetWeaponInfo()
    {
        return isUnlocked ? weaponSO : null; //nomes retorno l'arma si el slot esta desbloquejat
    }

    public void UnlockSlot() //no es una tenda pero una complicacio mes q vaig afegir es q els slots es puguin desbloquejar
    {
        isUnlocked = true;
        UpdateSlotTransparency();
    }

    public void LockSlot()
    {
        isUnlocked = false;
        UpdateSlotTransparency();
    }

    private void UpdateSlotTransparency()
    {
        if (itemImageComponent != null)
        {
            Color color = itemImageComponent.color;
            color.a = isUnlocked ? 1f : 0.2f;  //si esta desbloquejat la transparencia es 1, sino 0.2
            itemImageComponent.color = color;
        }
    }


}
