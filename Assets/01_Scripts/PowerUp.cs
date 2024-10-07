using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public PowerUpType type;
    public float amount = 0.5f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.ApplyPowerUp(type, amount);
            Destroy(gameObject);
        }
    }
}

public enum PowerUpType
{
    Damage,
    MoveSpeed,
    BulletSpeed,
    CriticalRate,
    FireRate,
    Shield
}