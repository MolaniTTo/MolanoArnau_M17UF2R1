using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject[] bulletPrefabs;

    public Transform[] firePoints;

    private Vector2[] directions = new Vector2[]
    {
        new Vector2(1,1),
        new Vector2(1,-1),
        new Vector2(-1,1),
        new Vector2(-1,-1)
    };

    public void Shoot()
    {

        for (int i=0; i<bulletPrefabs.Length; i++)
        {
            GameObject bullet = Instantiate(bulletPrefabs[i], firePoints[i].position, firePoints[i].rotation);
            bullet.GetComponent<BulletEnemy>().SetDirection(directions[i]);
        }
    }
}
