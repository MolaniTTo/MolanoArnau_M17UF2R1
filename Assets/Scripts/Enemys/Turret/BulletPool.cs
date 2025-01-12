using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public GameObject[] bulletPrefabs;  // Diferentes prefabs de balas
    public int poolSize = 10;  // Tamaño del pool para cada tipo de bala

    // Lista de pools para cada tipo de bala
    private List<GameObject>[] pools;

    private void Start()
    {
        // Inicializa el array de pools
        pools = new List<GameObject>[bulletPrefabs.Length];

        // Inicializa los pools para cada prefab de bala
        for (int i = 0; i < bulletPrefabs.Length; i++)
        {
            pools[i] = new List<GameObject>();

            // Rellena el pool con balas desactivadas
            for (int j = 0; j < poolSize; j++)
            {
                GameObject bullet = Instantiate(bulletPrefabs[i]);
                bullet.SetActive(false);  // Desactiva la bala en el pool
                pools[i].Add(bullet);
            }
        }
    }

    // Método para obtener una bala de un tipo específico
    public GameObject GetBullet(int index)
    {
        // Si el pool de este tipo de bala está vacío, crea una nueva
        if (pools[index].Count > 0)
        {
            GameObject bullet = pools[index][0];
            pools[index].RemoveAt(0);  // Elimina la bala del pool
            bullet.SetActive(true);  // Activa la bala
            return bullet;
        }
        else
        {
            // Si el pool está vacío, crea una nueva (opcional)
            GameObject bullet = Instantiate(bulletPrefabs[index]);
            return bullet;
        }
    }

    // Método para devolver una bala al pool
    public void ReturnBullet(int index, GameObject bullet)
    {
        bullet.SetActive(false);  // Desactiva la bala
        pools[index].Add(bullet);  // La añade al pool
        Debug.Log("Bullet returned to pool");
    }
}
