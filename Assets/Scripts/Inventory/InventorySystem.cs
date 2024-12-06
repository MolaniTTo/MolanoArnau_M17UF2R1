using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private int activeSlotIndex = 0;

    private PlayerInputActions playerInputActions;
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        playerInputActions.Inventory.Keyboard.performed += ctx => ChangeActiveSlot((int)ctx.ReadValue<float>());
        ChangeActiveHightLight(0);
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void ChangeActiveSlot(int slotIndex)
    {
        ChangeActiveHightLight(slotIndex - 1);
    }

    private void ChangeActiveHightLight(int slotIndex)
    {
        activeSlotIndex = slotIndex;
        foreach(Transform slot in this.transform)
        {
            slot.GetChild(0).gameObject.SetActive(false);
        }
        this.transform.GetChild(slotIndex).GetChild(0).gameObject.SetActive(true);
        //ChangeActiveWeapon();

    }

   /* private void ChangeActiveWeapon()
    {
        if(ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }

        if(!transform.GetChild(activeSlotIndex).GetComponentInChildren<InventorySlot>())
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }

        GameObject weaponToSpawn = transform.GetChild(activeSlotIndex).GetComponentInChildren<InventorySlot>().GetWeaponInfo().weaponPrefab;
        GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity);

    }*/

}
