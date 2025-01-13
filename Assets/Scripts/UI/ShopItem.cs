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
    private WeaponAudio weaponAudio;

    private BuyButtonController buyButtonController;

    private void Awake()
    {
        weaponAudio = FindObjectOfType<WeaponAudio>();
    }

    private void Start()
    {
        buyButtonController = FindObjectOfType<BuyButtonController>();
        if (interactionCanvas != null)
        {
            interactionCanvas.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) //quan el player entra en el trigger de l'objecte
    {
        if (other.CompareTag("Player") && interactionCanvas != null)
        {
            if (activeShopItem != null)
            {
                activeShopItem.DeactivateCanvas();
            }

            ActivateCanvas();
            activeShopItem = this;

            //mosta el botó de compra
            if (buyButtonController != null)
            {
                buyButtonController.ShowBuyButton(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && interactionCanvas != null) //quan el player surt del trigger de l'objecte
        {
            if (activeShopItem == this)
            {
                DeactivateCanvas();
                activeShopItem = null;

                //s'amaga el botó de compra
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
            interactionCanvas.SetActive(true); //activa el text del ítem
        }
    }

    private void DeactivateCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.SetActive(false); //desactiva el text del ítem
        }
    }

    public void BuyItem()
    {
        if (enemyCounter == null)
        {
            enemyCounter = FindObjectOfType<EnemyCounter>();
        }

        player = FindObjectOfType<PlayerController>(); 

        IWeapon activeWeapon = ActiveWeapon.Instance.CurrentActiveWeapon as IWeapon;

        WeaponSO weaponSO = ActiveWeapon.Instance.GetCurrentWeaponSO();

        sword = FindObjectOfType<Sword>(); 
     
        bow = FindObjectOfType<Bow>();
       
        weaponCollider = player.GetWeaponCollider();

        arrow = bow?.GetArrow();

        if (enemyCounter != null && enemyCounter.GetHeadCount() >= cost) //si tenim prous caps per comprar l'objecte
        {
            bool validPurchase = false; //compra vàlida

            switch (itemName)
            {
                case "DANZA DEL FILO":
                    if (activeWeapon is Sword)
                    {
                        weaponSO.weaponCooldown -= 0.1f;
                        ActiveWeapon.Instance.UpdateActiveWeaponProperties(); //actualitza les propietats de l'arma activa
                        validPurchase = true;

                    }
                    break;

                case "TENSION PERFECTA":
                    if (activeWeapon is Bow)
                    {
                        weaponSO.weaponCooldown -= 0.1f;
                        ActiveWeapon.Instance.UpdateActiveWeaponProperties(); //el cooldown de l'arma activa el baixa
                        validPurchase = true;
                    }
                    break;

                case "BENDICION VITAL":
                    player?.Heal(100); //cura el player
                    validPurchase = true;
                    break;

                case "MURO INQUEBRANTABLE":
                    player?.ActivateShield(); //activa el escut
                    validPurchase = true;
                    break;

                case "FLECHAS VENGADORAS":
                    if (activeWeapon is Bow)
                    {
                        Bow bow = activeWeapon as Bow;
                        bow.GetArrow()?.GetComponent<DamageSource>()?.IncreaseDamage(1f); //augmenta el damage de les fletxes
                        validPurchase = true;
                    }
                    break;

                case "CORTE IMPLACABLE":
                    if (activeWeapon is Sword)
                    {
                        weaponCollider?.GetComponent<DamageSource>()?.IncreaseDamage(1f); //augmenta el damage de l'espasa
                        validPurchase = true;
                    }
                    break;
            }

            if (validPurchase)
            {
                enemyCounter.DecreaseHeadCountGradually(cost);

                weaponAudio?.PlayPlayerSound(weaponAudio.Item);
            }
        }
    }
}
