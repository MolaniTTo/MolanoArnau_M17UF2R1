using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyButtonController : MonoBehaviour
{
    private GameObject buyButton; // El bot�n en la escena
    private TextMeshProUGUI buttonText; // El texto del bot�n
    private ShopItem currentShopItem; // El �tem actualmente activo en la tienda
    private EnemyCounter enemyCounter; // Referencia al contador de enemigos

    private void Start()
    {
        // Buscar el bot�n con el tag BuyButton
        buyButton = GameObject.FindGameObjectWithTag("BuyButton");
        if (buyButton == null)
        {
            Debug.LogError("No se encontr� el bot�n de compra con el tag 'BuyButton'.");
            return;
        }

        // Obtener el componente TextMeshPro del texto del bot�n
        buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
            Debug.LogError("No se encontr� un componente TextMeshProUGUI en el bot�n de compra.");
        }

        // Configurar el evento onClick del bot�n
        Button buttonComponent = buyButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(OnBuyButtonClicked);
        }
        else
        {
            Debug.LogError("El bot�n no tiene un componente Button.");
        }

        // Iniciar el bot�n como invisible
        HideBuyButton();

        enemyCounter = FindObjectOfType<EnemyCounter>();
        if (enemyCounter == null)
        {
            Debug.LogError("No se encontr� un EnemyCounter en la escena.");
        }
    }

    public void ShowBuyButton(ShopItem shopItem)
    {
        currentShopItem = shopItem; // Asignar el �tem activo

        if (buyButton != null)
        {
            // Hacer visible la imagen del bot�n
            Image buttonImage = buyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.enabled = true;
            }

            // Hacer visible el texto del bot�n
            if (buttonText != null)
            {
                buttonText.enabled = true;
                buttonText.text = $"Comprar {shopItem.itemName} ({shopItem.cost})";
            }

            // Activar la interacci�n del bot�n
            Button buttonComponent = buyButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = true;
            }
        }
    }

    public void HideBuyButton()
    {
        currentShopItem = null; // Limpiar el �tem activo

        if (buyButton != null)
        {
            // Ocultar la imagen del bot�n
            Image buttonImage = buyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.enabled = false;
            }

            // Ocultar el texto del bot�n
            if (buttonText != null)
            {
                buttonText.enabled = false;
            }

            // Desactivar la interacci�n del bot�n
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
                currentShopItem.BuyItem(); // Llamar al m�todo de compra en el �tem activo
                UpdateButtonText(); // Actualizar el texto despu�s de la compra
            }
            else
            {
                // Mostrar mensaje de advertencia en el bot�n
                buttonText.text = warningMessage;
            }
        }
    }


    private bool CanPurchaseItem(out string warningMessage)
    {
        warningMessage = string.Empty;

        // Verificar si hay suficiente dinero
        if (enemyCounter.GetHeadCount() < currentShopItem.cost)
        {
            warningMessage = "No tienes suficientes cabezas.";
            return false;
        }

        // Verificar si el arma activa es compatible
        IWeapon activeWeapon = ActiveWeapon.Instance.CurrentActiveWeapon as IWeapon;
        if (currentShopItem.itemName == "DANZA DEL FILO" && !(activeWeapon is Sword))
        {
            warningMessage = "Equipa la espada para mejorar su cooldown.";
            return false;
        }

        if (currentShopItem.itemName == "TENSION PERFECTA" && !(activeWeapon is Bow))
        {
            warningMessage = "Equipa el arco para mejorar su cooldown.";
            return false;
        }

        // Otros casos se pueden agregar aqu�

        return true;
    }

    private void UpdateButtonText()
    {
        if (buttonText != null && currentShopItem != null)
        {
            buttonText.text = $"Comprar {currentShopItem.itemName} ({currentShopItem.cost})";
        }
    }
}
