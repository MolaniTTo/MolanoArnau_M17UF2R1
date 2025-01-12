using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuyButtonController : MonoBehaviour
{
    private GameObject buyButton; // El botón en la escena
    private TextMeshProUGUI buttonText; // El texto del botón
    private ShopItem currentShopItem; // El ítem actualmente activo en la tienda
    private EnemyCounter enemyCounter; // Referencia al contador de enemigos

    private void Start()
    {
        // Buscar el botón con el tag BuyButton
        buyButton = GameObject.FindGameObjectWithTag("BuyButton");
        if (buyButton == null)
        {
            Debug.LogError("No se encontró el botón de compra con el tag 'BuyButton'.");
            return;
        }

        // Obtener el componente TextMeshPro del texto del botón
        buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null)
        {
            Debug.LogError("No se encontró un componente TextMeshProUGUI en el botón de compra.");
        }

        // Configurar el evento onClick del botón
        Button buttonComponent = buyButton.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(OnBuyButtonClicked);
        }
        else
        {
            Debug.LogError("El botón no tiene un componente Button.");
        }

        // Iniciar el botón como invisible
        HideBuyButton();

        enemyCounter = FindObjectOfType<EnemyCounter>();
        if (enemyCounter == null)
        {
            Debug.LogError("No se encontró un EnemyCounter en la escena.");
        }
    }

    public void ShowBuyButton(ShopItem shopItem)
    {
        currentShopItem = shopItem; // Asignar el ítem activo

        if (buyButton != null)
        {
            // Hacer visible la imagen del botón
            Image buttonImage = buyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.enabled = true;
            }

            // Hacer visible el texto del botón
            if (buttonText != null)
            {
                buttonText.enabled = true;
                buttonText.text = $"Comprar {shopItem.itemName} ({shopItem.cost})";
            }

            // Activar la interacción del botón
            Button buttonComponent = buyButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.interactable = true;
            }
        }
    }

    public void HideBuyButton()
    {
        currentShopItem = null; // Limpiar el ítem activo

        if (buyButton != null)
        {
            // Ocultar la imagen del botón
            Image buttonImage = buyButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.enabled = false;
            }

            // Ocultar el texto del botón
            if (buttonText != null)
            {
                buttonText.enabled = false;
            }

            // Desactivar la interacción del botón
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
                currentShopItem.BuyItem(); // Llamar al método de compra en el ítem activo
                UpdateButtonText(); // Actualizar el texto después de la compra
            }
            else
            {
                // Mostrar mensaje de advertencia en el botón
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

        // Otros casos se pueden agregar aquí

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
