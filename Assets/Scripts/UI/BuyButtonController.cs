using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyButtonController : MonoBehaviour
{
    private GameObject buyButton; //boto de compra
    private TextMeshProUGUI buttonText; //text del boto de compra
    private ShopItem currentShopItem; //item de la botiga actual
    private EnemyCounter enemyCounter; //referencia al contador d'enemics

    private void Start()
    {
        buyButton = GameObject.FindGameObjectWithTag("BuyButton");
        if (buyButton == null)
        {
            return;
        }

        //component de text del boto
        buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();

        //configuracio del onClick del boto
        Button buttonComponent = buyButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(OnBuyButtonClicked);
        }

        //iniciem com a ocult
        HideBuyButton();
        enemyCounter = FindObjectOfType<EnemyCounter>();
    }

    public void ShowBuyButton(ShopItem shopItem)
    {
        currentShopItem = shopItem; //li passem el item actual

        if (buyButton != null)
        {
            //fer visible la imatge del boto
            Image buttonImage = buyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.enabled = true;
            }

            //fer visible el text del boto
            if (buttonText != null)
            {
                buttonText.enabled = true;
                buttonText.text = $"Comprar {shopItem.itemName} ({shopItem.cost})";
            }

            //activar la interacciodel boto
            Button buttonComponent = buyButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = true;
            }
        }
    }

    public void HideBuyButton()
    {
        currentShopItem = null; //borrem el item actual

        if (buyButton != null)
        {
            //ocultem la imatge del boto
            Image buttonImage = buyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.enabled = false;
            }

            //ocultem text
            if (buttonText != null)
            {
                buttonText.enabled = false;
            }

            //desactivar la interacció del boto
            Button buttonComponent = buyButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = false;
            }
        }
    }

    private void OnBuyButtonClicked()
    {
        if (currentShopItem != null)
        {
            if (CanPurchaseItem(out string warningMessage))
            {
                currentShopItem.BuyItem(); //cridem al metode de compra
                UpdateButtonText(); //actualitzar les dades de compra
            }
            else
            {
                //mostrar missatge d'error
                buttonText.text = warningMessage;
            }
        }
    }


    private bool CanPurchaseItem(out string warningMessage)
    {
        warningMessage = string.Empty;

        //mirar si tenim sufieicents caps
        if (enemyCounter.GetHeadCount() < currentShopItem.cost)
        {
            warningMessage = "No tienes suficientes cabezas.";
            return false;
        }

        // mirem si l'arma es compatible amb l'item que volem millorar
        IWeapon activeWeapon = ActiveWeapon.Instance.CurrentActiveWeapon as IWeapon;
        if ((currentShopItem.itemName == "DANZA DEL FILO" || currentShopItem.itemName == "CORTE IMPLACABLE") && !(activeWeapon is Sword))
        {
            warningMessage = "Equipa la espada para poder mejorarla.";
            return false;
        }

        if ((currentShopItem.itemName == "TENSION PERFECTA" || currentShopItem.itemName == "FLECHAS VENGADORAS") && !(activeWeapon is Bow))
        {
            warningMessage = "Equipa el arco para mejorarlo.";
            return false;
        }

        return true;
    }

    private void UpdateButtonText() //actualitzar el text del boto
    {
        if (buttonText != null && currentShopItem != null)
        {
            buttonText.text = $"Comprar {currentShopItem.itemName} ({currentShopItem.cost})";
        }
    }
}
