using UnityEngine;

public class CloudParallax : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public float parallaxFactor = 0.5f; // Velocidad del efecto parallax (menor a 1 = más lento)
    private Vector3 lastPlayerPosition; // Última posición conocida del jugador
    private bool isPlayerAssigned = false; // Verificar si el jugador está asignado

    private void Start()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
            isPlayerAssigned = true;
        }
    }

    private void Update()
    {
        if (player == null || !isPlayerAssigned) return;

        // Calcular el cambio en la posición del jugador
        Vector3 playerDelta = player.position - lastPlayerPosition;

        // Mover las nubes en dirección opuesta al movimiento del jugador
        transform.position -= new Vector3(playerDelta.x * parallaxFactor, playerDelta.y * parallaxFactor, 0);

        // Actualizar la última posición del jugador
        lastPlayerPosition = player.position;
    }

    // Método público para asignar dinámicamente el jugador
    public void AssignPlayer(Transform playerTransform)
    {
        player = playerTransform;
        lastPlayerPosition = player.position;
        isPlayerAssigned = true;
    }
}
