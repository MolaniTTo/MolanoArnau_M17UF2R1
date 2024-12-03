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

    }

}
