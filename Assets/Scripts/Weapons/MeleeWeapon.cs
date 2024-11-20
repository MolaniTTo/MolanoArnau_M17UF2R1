using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public Transform player;
    public float rotationOffset = 90f; // Offset para ajustar la orientaci�n del sprite
    public float weaponDistance = 1.5f;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();   
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            if(spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                Debug.Log("Sword enabled");
            }

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Aseg�rate de que la posici�n Z est� en 0 para 2D

            if(Vector3.Distance(player.position, mousePosition) > weaponDistance)
            {
                RotateSwordToCursor();
                UpdateSwordPosition();
                UpdateSwordLayer(mousePosition);
            }
        }
        else
        {
            if(spriteRenderer != null)
                spriteRenderer.enabled = false;
        }
    }

    void RotateSwordToCursor()
    {
        // Obt�n la posici�n del mouse en el mundo
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = 0; // Aseg�rate de que la posici�n Z est� en 0 para 2D

        // Calcula la direcci�n desde el objeto hacia el mouse
        Vector3 direction = mousePosition - transform.position;

        // Calcula el �ngulo hacia el mouse en grados
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica la rotaci�n al objeto
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - rotationOffset));

    }

    void UpdateSwordPosition()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Aseg�rate de que la posici�n Z est� en 0 para 2D

        Vector3 direction = (mousePosition - player.position).normalized;

        transform.position = player.position + direction * weaponDistance;

    }

    private void UpdateSwordLayer(Vector3 mousePosition)
    {
        if (spriteRenderer == null) return;

        // Calcula la direcci�n desde el jugador hacia el mouse
        Vector3 direction = mousePosition - player.position;

        // Calcula el �ngulo hacia el mouse en grados
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplica el rotationOffset al c�lculo del �ngulo
        angle += rotationOffset;

        // Asegura que el �ngulo sea siempre positivo (0� a 360�)
        if (angle < 0)
        {
            angle += 360f;
        }

        // Cambia el sortingOrder basado en el cuadrante
        if (angle >= 0 && angle < 90 || angle >= 270 && angle < 360)
        {
            // Primer y Cuarto cuadrante: delante del jugador
            spriteRenderer.sortingOrder = 11; // Capa detr�s
        }
        else
        {
            // Segundo y Tercer cuadrante: detras del jugador
            spriteRenderer.sortingOrder = 9; // Capa delante
        }
    }

}
