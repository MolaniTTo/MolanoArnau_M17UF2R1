using UnityEngine;

public class ShopItem : MonoBehaviour
{
    public GameObject interactionCanvas; // Texto del ítem
    public int cost;
    public string itemName;

    private static ShopItem activeShopItem; // Ítem activo
    private EnemyCounter enemyCounter;
    private Sword sword;
    private Bow bow;
    private Transform weaponCollider;
    private GameObject arrow;
    private PlayerController player;

    private BuyButtonController buyButtonController;

    private void Start()
    {
        buyButtonController = FindObjectOfType<BuyButtonController>();
        if (interactionCanvas != null)
        {
            interactionCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactionCanvas != null)
        {
            if (activeShopItem != null)
            {
                activeShopItem.DeactivateCanvas();
            }

            ActivateCanvas();
            activeShopItem = this;

            // Mostrar el botón de compra
            if (buyButtonController != null)
            {
                buyButtonController.ShowBuyButton(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactionCanvas != null)
        {
            if (activeShopItem == this)
            {
                DeactivateCanvas();
                activeShopItem = null;

                // Ocultar el botón de compra
                if (buyButtonController != null)
                {
                    buyButtonController.HideBuyButton();
                }
            }
        }
    }

    private void ActivateCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.SetActive(true);
        }
    }

    private void DeactivateCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.SetActive(false);
        }
    }

    public void BuyItem()
    {
        if (enemyCounter == null)
        {
            enemyCounter = FindObjectOfType<EnemyCounter>();
        }


        player = FindObjectOfType<PlayerController>(); 
        if (player == null)
        {
            Debug.Log("Player found");
            return;
        }

        IWeapon activeWeapon = ActiveWeapon.Instance.CurrentActiveWeapon as IWeapon;
        if (activeWeapon == null)
        {
            Debug.Log("Active weapon found");
            return;
        }

        WeaponSO weaponSO = ActiveWeapon.Instance.GetCurrentWeaponSO();

        //la sword
        sword = FindObjectOfType<Sword>(); 
        if(sword != null)
        {
            Debug.Log("Sword found");
        }
        //la bow
        bow = FindObjectOfType<Bow>();
        if (bow != null)
        {
            Debug.Log("Bow found");
        }

        //el collider de la espada que esta dentro del player dentro de un objeto con el componente activeWeapon
        weaponCollider = player.GetWeaponCollider();
        if (weaponCollider != null)
        {
            Debug.Log("WeaponCollider found");
        }

        //el collider de la flecha que es un prefab que se instancia cuando disparamos con el arco
        arrow = bow?.GetArrow();
        if (arrow != null)
        {
            Debug.Log("Arrow found");
        }


        if (enemyCounter != null && enemyCounter.GetHeadCount() >= cost)
        {
            bool validPurchase = false;

            switch (itemName)
            {
                case "DANZA DEL FILO":
                    if (activeWeapon is Sword)
                    {
                        Debug.Log("Sword cooldown decreased");
                        weaponSO.weaponCooldown -= 0.1f;
                        ActiveWeapon.Instance.UpdateActiveWeaponProperties();
                        validPurchase = true;

                    }
                    else
                    {
                        Debug.LogError("Cannot buy sword cooldown: Active weapon is not a sword.");
                    }
                    break;

                case "TENSION PERFECTA":
                    if (activeWeapon is Bow)
                    {
                        Debug.Log("Bow cooldown decreased");
                        weaponSO.weaponCooldown -= 0.1f;
                        ActiveWeapon.Instance.UpdateActiveWeaponProperties();
                        validPurchase = true;
                    }
                    else
                    {
                        Debug.LogError("Cannot buy bow cooldown: Active weapon is not a bow.");
                    }
                    break;

                case "BENDICION VITAL":
                    Debug.Log("Player healed");
                    player?.Heal(100);
                    validPurchase = true;
                    break;

                case "MURO INQUEBRANTABLE":
                    Debug.Log("Player shield");
                    player?.ActivateShield();
                    validPurchase = true;
                    break;

                case "FLECHAS VENGADORAS":
                    if (activeWeapon is Bow)
                    {
                        Debug.Log("Bow damage increased");
                        Bow bow = activeWeapon as Bow;
                        bow.GetArrow()?.GetComponent<DamageSource>()?.IncreaseDamage(1f);
                        validPurchase = true;
                    }
                    else
                    {
                        Debug.LogError("Cannot buy bow damage: Active weapon is not a bow.");
                    }
                    break;

                case "CORTE IMPLACABLE":
                    if (activeWeapon is Sword)
                    {
                        Debug.Log("Sword damage increased");
                        weaponCollider?.GetComponent<DamageSource>()?.IncreaseDamage(1f);
                        validPurchase = true;
                    }
                    else
                    {
                        Debug.LogError("Cannot buy sword damage: Active weapon is not a sword.");
                    }
                    break;
            }

            if (validPurchase)
            {
                enemyCounter.DecreaseHeadCountGradually(cost);
            }
            else
            {
                Debug.LogError($"Purchase failed: Not enough conditions met for item '{itemName}'");
            }
        }
    }
}
