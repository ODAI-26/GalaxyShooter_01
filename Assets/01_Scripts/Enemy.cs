using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public EnemyType type;
    public float maxLife = 3;
    float life = 3;
    public float speed = 2;
    public float timeBtwShoot = 1.5f;
    public float damage = 1f;
    public float bulletSpeed = 4;
    public int scorePoints = 1;
    float timer = 0;
    public float range = 4;
    bool targetInRange = false;
    Transform target;
    public Transform firePoint;
    public Bullet bulletPrefab;
    public float dropChance = 30f;
    public List<GameObject> powerUps = new List<GameObject>();
    public GameObject explosionEffect;
    public Image lifeBar;
    

    void Start()
    {
        GameObject ship = GameObject.FindGameObjectWithTag("Player");
        target = ship.transform;
        life = maxLife;
        lifeBar.fillAmount = life / maxLife;
    }

    void Update()
    {
        switch (type)
        {
            case EnemyType.Normal:
                MoveForward();
                break;
            case EnemyType.NormalShoot:
                MoveForward();
                Shoot();
                break;
            case EnemyType.Kamikase:
                if (targetInRange)
                {
                    RotateToTarget();
                    MoveForward(2);
                }
                else
                {
                    MoveForward();
                    SearchTarget();
                }
                break;
            case EnemyType.Sniper:
                if (targetInRange)
                {
                    RotateToTarget();
                    Shoot();
                }
                else
                {
                    MoveForward();
                    SearchTarget();
                }
                break;
        }
    }

    public void TakeDamage(float dmg)
    {
        life -= dmg;
        lifeBar.fillAmount = life / maxLife;
        if (life <= 0)
        {
            if(Random.Range(0f,100) <= dropChance)
            {
                Instantiate(powerUps[Random.Range(0, powerUps.Count)],
                    transform.position, transform.rotation);
            }
            Instantiate(explosionEffect, transform.position, transform.rotation);
            //Spawner.instance.AddScore(scorePoints);
            Spawner.instance.AddScore(10);
            Destroy(gameObject);
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void MoveForward(float m)
    {
        transform.Translate(Vector3.forward * speed * m * Time.deltaTime);
    }

    void RotateToTarget()
    {
        Vector3 dir = target.position - transform.position;
        float angleY = Mathf.Atan2 (dir.x, dir.z) * Mathf.Rad2Deg + 0;
        transform.rotation = Quaternion.Euler(0, angleY, 0);
    }

    void SearchTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if(distance <= range)
        {
            targetInRange = true;
        }
        else
        {
            targetInRange = false;
        }
    }

    void Shoot()
    {
        if (timer < timeBtwShoot)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            Bullet b = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            b.speed = bulletSpeed;
            b.damage = damage;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.TakeDamage(damage);
            if (Random.Range(0f, 100) <= dropChance)
            {
                Instantiate(powerUps[Random.Range(0, powerUps.Count)],
                    transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }else if(collision.gameObject.CompareTag("Destroyer"))
        {
            Destroy(gameObject);
        }
    }
}
public enum EnemyType
{
    Normal,
    NormalShoot,
    Kamikase,
    Sniper
}