using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    private int activeSlotIndexNum = 0;

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
    }

    private void Start()
    {
        StartCoroutine(InitializeInventory());
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void ToggleActiveSlot(int numValue)
    {
        ToggleActiveHighlight(numValue-1);
    }

    private IEnumerator InitializeInventory()
    {
        // Esperar un frame para asegurarse de que todos los objetos han sido instanciados
        yield return null;

        if (GetComponentInChildren<InventorySlot>() == null || GetComponentInChildren<InventorySlot>().GetWeaponInfo() == null)
        {
            Debug.LogError("No weapons are assigned in the inventory slots!");
            yield break;
        }

        playerInputActions.Inventory.Keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());
        ToggleActiveHighlight(0); // Inicializar el arma del primer slot
    }



    private void ToggleActiveHighlight(int indexNum)
    {
        if (indexNum < 0 || indexNum >= transform.childCount)
        {
            Debug.LogError($"Invalid inventory slot index: {indexNum}");
            return;
        }

        activeSlotIndexNum = indexNum;

        foreach (Transform inventorySlot in this.transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }

        Transform selectedSlot = transform.GetChild(indexNum);
        selectedSlot.GetChild(0).gameObject.SetActive(true);

        ChangeActiveWeapon();

    }

    private void ChangeActiveWeapon()
    {
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }

        // Validar si el slot activo tiene un InventorySlot con WeaponSO
        var activeSlot = transform.GetChild(activeSlotIndexNum).GetComponentInChildren<InventorySlot>();
        if (activeSlot == null || activeSlot.GetWeaponInfo() == null)
        {
            ActiveWeapon.Instance.WeaponNull();
            Debug.LogWarning("No weapon assigned to the active slot!");
            return;
        }

        // Instanciar el arma
        GameObject weaponToSpawn = activeSlot.GetWeaponInfo().weaponPrefab;
        GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity);
        ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        newWeapon.transform.parent = ActiveWeapon.Instance.transform;
        newWeapon.transform.localScale = new Vector3(0.779029965f, 0.779029965f, 0);


        // Configurar el arma como la activa
        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());

    }
}
