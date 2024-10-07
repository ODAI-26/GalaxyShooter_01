using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float damage = 1f;
    public float bulletSpeed = 7;
    public float speed = 2f;
    public float timeBtwShoot = 1.5f;
    public float maxLife = 3;
    public float life = 3;
    public int ammo = 5;
    public float criticalChance = 0f;
    int currentAmmo;
    float timer = 0;
    bool canShoot = true;
    public bool shield = false;
    public float shieldTime = 5f;
    public Rigidbody rb;
    public Transform firePoint;
    public Bullet bulletPrefab;
    public Text lifeText;
    public Image lifeBar;

    void Start()
    {
        Debug.Log("Inició el juego");
        currentAmmo = ammo;
        life = maxLife;
        lifeBar.fillAmount = life / maxLife;
        lifeText.text = "Life = " + life;
    }

    void Update()
    {
        Debug.Log("Juego en proceso");
        Movement();
        Reload();
        CheckIfCanShoot();
        Shoot();
    }

    public void TakeDamage(float dmg)
    {
        if (!shield)
        {
            life -= dmg;
            lifeBar.fillAmount = life / maxLife;
            lifeText.text = "Life = " + life;
            if (life <= 0)
            {
                SceneManager.LoadScene("Game");
                //Destroy(gameObject);
            }
        }
    }

    void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        rb.velocity = new Vector3(x, 0, z) * speed;
    }

    void Shoot()
    {
        if(Input.GetKeyDown(KeyCode.Space) && canShoot && currentAmmo > 0)
        {
            Bullet b = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            if(Random.Range(0f, 1f) <= criticalChance)
            {
                b.damage = damage * 2;
            }
            else
            {
                b.damage = damage;
            }
            b.speed = bulletSpeed;
            currentAmmo--;
            canShoot = false;
        }
    }

    void Reload()
    {
        if(currentAmmo == 0 && Input.GetKeyDown(KeyCode.R))
        {
            currentAmmo = ammo;
        }
    }

    void CheckIfCanShoot()
    {
        if (timer < timeBtwShoot)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            canShoot = true;
        }
    }

    public void ApplyPowerUp(PowerUpType powerUp, float amount)
    {
        switch (powerUp)
        {
            case PowerUpType.Damage:
                damage += amount;
                break;
            case PowerUpType.MoveSpeed:
                speed += amount;
                break;
            case PowerUpType.BulletSpeed:
                bulletSpeed += amount;
                break;
            case PowerUpType.CriticalRate:
                criticalChance = amount;//0.3f
                break;
            case PowerUpType.FireRate:
                timeBtwShoot -= amount;
                if(timeBtwShoot <= 0)
                {
                    timeBtwShoot = 0.1f;
                }
                break;
            case PowerUpType.Shield:
                StartCoroutine(ApplyShield());
                break;
        }
    }

    IEnumerator ApplyShield()
    {
        shield = true;
        yield return new WaitForSeconds(shieldTime);
        shield = false;
    }
}