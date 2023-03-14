using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectileScript : ProjectileScript
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().takeDamage(damage,owner);
        }
        Destroy(gameObject);
    }
}
