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

    public void ToggleActiveSlot(int numValue) // es per canviar l'arma amb els números
    {
        if(transform.GetChild(numValue-1).GetComponentInChildren<InventorySlot>().GetWeaponInfo() == null)
        {
            return;
        }

        ToggleActiveHighlight(numValue-1);
    }

    public void UnlockSlot(int index) //desbloquejar slot
    {
        if (index >= 0 && index < transform.childCount)
        {
            var slot = transform.GetChild(index).GetComponentInChildren<InventorySlot>();
            if (slot != null)
            {
                slot.UnlockSlot();
            }
        }
    }

    public void ResetSlots() //bloquejar tots els slots menys el primer
    {
        var slots = GetComponentsInChildren<InventorySlot>();
        foreach (var slot in slots)
        {
            if (slot != slots[0])
            {
                slot.LockSlot();
            }
        }
    }

    private IEnumerator InitializeInventory()
    {
        //s'espera un frame perquè els slots s'hagin inicialitzat
        yield return new WaitForSeconds(3.5f);  

        if (GetComponentInChildren<InventorySlot>() == null || GetComponentInChildren<InventorySlot>().GetWeaponInfo() == null)
        {
            yield break;
        }

        playerInputActions.Inventory.Keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());
        ToggleActiveHighlight(0); //inicialitzar el primer slot com a actiu
    }



    private void ToggleActiveHighlight(int indexNum)
    {
        if (indexNum < 0 || indexNum >= transform.childCount)
        {
            return;
        }

        activeSlotIndexNum = indexNum;

        foreach (Transform inventorySlot in this.transform) //desactivar tots els highlights
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }

        Transform selectedSlot = transform.GetChild(indexNum);
        selectedSlot.GetChild(0).gameObject.SetActive(true); //activar el slot 0, pq sino ens podem quedar amb el 1 a la ma i no s'esta bloquejant be

        ChangeActiveWeapon();

    }

    private void ChangeActiveWeapon() //canviar l'arma activa
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
            return;
        }

        //instanciem l'arma
        GameObject weaponToSpawn = activeSlot.GetWeaponInfo().weaponPrefab;
        GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity);
        ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        newWeapon.transform.parent = ActiveWeapon.Instance.transform;
        newWeapon.transform.localScale = new Vector3(0.779029965f, 0.779029965f, 0);


        //la posem com a arma activa
        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());

    }

    public void Destroy()//es pq em donava un error, no ho utilitzo creo pero no ho borrare pq no recordo ni si ho crido a cap puesto (pero crec qno)
    {
        Destroy(gameObject);
    }
}
