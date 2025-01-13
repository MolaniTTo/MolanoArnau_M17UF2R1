using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public GameObject[] bulletPrefabs;  // Prefabs de les bales
    public int poolSize = 10;  //tamany de la pool

    //llista de llistes de gameobjects per a cada tipus de bala
    private List<GameObject>[] pools;

    private void Start()
    {
        //inicialitza l'array de pools
        pools = new List<GameObject>[bulletPrefabs.Length];

        //inicialitza les pools per a cada tipus de bala
        for (int i = 0; i < bulletPrefabs.Length; i++)
        {
            pools[i] = new List<GameObject>();

            //omple la pool amb les bales
            for (int j = 0; j < poolSize; j++)
            {
                GameObject bullet = Instantiate(bulletPrefabs[i]);
                bullet.SetActive(false);  //Desactiva la bala
                pools[i].Add(bullet);
            }
        }
    }

    // Mètode per obtenir una bala del pool
    public GameObject GetBullet(int index)
    {
        //si la pool no està buida, retorna una bala
        if (pools[index].Count > 0)
        {
            GameObject bullet = pools[index][0];
            pools[index].RemoveAt(0);  // Elimina la bala de la pool
            bullet.SetActive(true);  // Activa la bala
            return bullet;
        }
        else
        {
            //si la pool està buida, crea una nova bala
            GameObject bullet = Instantiate(bulletPrefabs[index]);
            return bullet;
        }
    }

    //metode per retornar una bala al pool
    public void ReturnBullet(int index, GameObject bullet)
    {
        bullet.SetActive(false);  // Desactiva la bala
        pools[index].Add(bullet);  // L'afegeix a la pool
        Debug.Log("Bullet returned to pool");
    }
}
