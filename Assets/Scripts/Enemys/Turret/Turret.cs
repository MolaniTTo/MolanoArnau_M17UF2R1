using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public GameObject[] bulletPrefabs;

    public Transform[] firePoints;
    public Transform Player;
    public float minDistance = 15f;
    public Animator myAnimator;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private Vector2[] directions = new Vector2[]
    {
        new Vector2(1,1),
        new Vector2(1,-1),
        new Vector2(-1,1),
        new Vector2(-1,-1)
    };

    public void IsInArea()
    {
        if (Vector2.Distance(transform.position, Player.position) < minDistance)
        {
            myAnimator.SetTrigger("Attack");
        }
    }

    public void Shoot()
    {

        for (int i=0; i<bulletPrefabs.Length; i++)
        {
            GameObject bullet = Instantiate(bulletPrefabs[i], firePoints[i].position, firePoints[i].rotation);
            bullet.GetComponent<BulletEnemy>().SetDirection(directions[i]);
        }
    }
}
