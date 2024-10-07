using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss : MonoBehaviour
{
    public float maxLife = 50;
    private float currentLife;
    public float horizontalMoveSpeed = 3f;
    public float verticalMoveSpeed = 2f;
    public float damage = 1f;
    public float bulletSpeed = 5f;
    public Transform[] firePoints; 
    public Bullet bulletPrefab;
    public int scorePoints = 2;
    public GameObject explosionEffect;

    private float shootCooldown = 1.5f; 
    private float shootTimer = 0f; 
    private float teleportCooldown = 2.0f;
    private float teleportTimer = 0f;

    private bool isPhaseOneComplete = false;
    private bool isPhaseTwoComplete = false; 
    private bool isPhaseThreeComplete = false; 
    private bool isPhaseFourComplete = false; 
    private bool isPhaseFive = false; 
    private Vector3 targetPosition; 
    private bool isShootingBurst = false;
    private bool movingRight = true;

    void Start()
    {
        currentLife = maxLife;
        PositionAtTopCenter();
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    void PositionAtTopCenter()
    {
        float screenCenterX = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0, Camera.main.nearClipPlane)).x;
        float screenTopY = Camera.main.ViewportToWorldPoint(new Vector3(0, 1f, Camera.main.nearClipPlane)).y;

        transform.position = new Vector3(screenCenterX, screenTopY - 1f, 0); 
    }

    void HandleMovement()
    {
        if (currentLife > 40) // Fase 1: Movimiento hacia abajo hasta detenerse
        {
            float stoppingYPosition = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.8f, Camera.main.nearClipPlane)).y;

            if (transform.position.y > stoppingYPosition)
            {
                transform.position += Vector3.down * verticalMoveSpeed * Time.deltaTime;
            }
            else
            {
                isPhaseOneComplete = true; 
                isPhaseTwoComplete = true; 
            }
        }
        else if (isPhaseTwoComplete && currentLife > 30) // Fase 2: Movimiento horizontal
        {
            float leftBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0, Camera.main.nearClipPlane)).x; 
            float rightBoundary = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0, Camera.main.nearClipPlane)).x; 

            MoveHorizontally(leftBoundary, rightBoundary);
        }
        else if (currentLife > 20 && isPhaseTwoComplete) // Fase 3: Movimientos rápidos aleatorios
        {
            if (!isPhaseThreeComplete)
            {
                isPhaseThreeComplete = true;
                SetRandomTargetPositionX(); 
            }
            MoveToTargetPosition(horizontalMoveSpeed*2);
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetRandomTargetPositionX();
            }
        }
        else if (currentLife > 10 && isPhaseThreeComplete) // Fase 4: Movimiento aleatorio en X y Y
        {
            if (!isPhaseFourComplete)
            {
                isPhaseFourComplete = true;
                SetRandomTargetPositionPhase4();
            }
            MoveToTargetPosition(horizontalMoveSpeed * 2); 
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetRandomTargetPositionPhase4();
            }
        }
        else if (currentLife <= 10 && isPhaseFourComplete) // Fase 5: Teletransportación y disparos en ráfaga en 360
        {
            if (!isPhaseFive)
            {
                isPhaseFive = true;
            }
            HandleTeleportation();
        }
    }
    void HandleTeleportation()
    {
        teleportTimer += Time.deltaTime; 

        if (teleportTimer >= teleportCooldown) 
        {
            float randomX = Random.Range(-5f, 5f); 
            float minY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, Camera.main.nearClipPlane)).y; 
            float maxY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.8f, Camera.main.nearClipPlane)).y; 
            float randomY = Random.Range(minY, maxY); 

            transform.position = new Vector3(randomX, randomY, 0);

            teleportTimer = 0f; 
            StartCoroutine(ShootAllDirections());
        }
    }
    void MoveHorizontally(float leftBoundary, float rightBoundary)
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * horizontalMoveSpeed * Time.deltaTime);
            if (transform.position.x >= rightBoundary)
                movingRight = false; 
        }
        else
        {
            transform.Translate(Vector3.left * horizontalMoveSpeed * Time.deltaTime);
            if (transform.position.x <= leftBoundary)
                movingRight = true; 
        }
    }
    void MoveToTargetPosition(float speed)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    void HandleShooting()
    {
        shootTimer += Time.deltaTime; 

        if (currentLife > 40 && isPhaseOneComplete) // Fase 1: Disparo recto
        {
            if (shootTimer >= shootCooldown)
            {
                Shoot(firePoints[0], firePoints[0].rotation);
                Shoot(firePoints[2], firePoints[2].rotation);
                Shoot(firePoints[4], firePoints[4].rotation);
                shootTimer = 0f; 
            }
        }

        if (isPhaseTwoComplete && currentLife > 30) // Fase 2: Disparo en cono
        {
            if (shootTimer >= shootCooldown)
            {
                Shoot(firePoints[0], Quaternion.Euler(0, 0, 195)); 
                Shoot(firePoints[2], Quaternion.Euler(0, 0, 180)); 
                Shoot(firePoints[4], Quaternion.Euler(0, 0, 165)); 

                shootTimer = 0f; 
            }
        }

        if (isPhaseThreeComplete && currentLife > 20) // Fase 3: Ráfaga de disparos
        {
            if (!isShootingBurst && shootTimer >= shootCooldown)
            {
                StartCoroutine(ShootBurst());
                shootTimer = 0f; 
            }
        }
        if (isPhaseFourComplete && currentLife > 10) // Fase 4: Disparo en todas las direcciones
        {
            if (shootTimer >= 3.0f) 
            {
                Shoot(firePoints[0], Quaternion.Euler(0, 0, 90)); 
                Shoot(firePoints[1], Quaternion.Euler(0, 0, 135));
                Shoot(firePoints[2], Quaternion.Euler(0, 0, 180)); 
                Shoot(firePoints[3], Quaternion.Euler(0, 0, 225));
                Shoot(firePoints[4], Quaternion.Euler(0, 0, 270));  
                Shoot(firePoints[5], Quaternion.Euler(0, 0, 315));
                Shoot(firePoints[6], Quaternion.Euler(0, 0, 0));
                Shoot(firePoints[7], Quaternion.Euler(0, 0, 45));

                shootTimer = 0f; 
            }
        }
    }

    IEnumerator ShootBurst()
    {
        isShootingBurst = true; 

        for (int i = 0; i < 5; i++) 
        {
            Shoot(firePoints[0], Quaternion.Euler(0, 0, 195)); 
            Shoot(firePoints[2], Quaternion.Euler(0, 0, 180)); 
            Shoot(firePoints[4], Quaternion.Euler(0, 0, 165)); 

            yield return new WaitForSeconds(0.1f); 
        }

        isShootingBurst = false;
    }
    IEnumerator ShootAllDirections()
    {
        for (int burstCount = 0; burstCount < 3; burstCount++)
        {
            Shoot(firePoints[0], Quaternion.Euler(0, 0, 90)); 
            Shoot(firePoints[1], Quaternion.Euler(0, 0, 135));
            Shoot(firePoints[2], Quaternion.Euler(0, 0, 180)); 
            Shoot(firePoints[3], Quaternion.Euler(0, 0, 225));
            Shoot(firePoints[4], Quaternion.Euler(0, 0, 270));  
            Shoot(firePoints[5], Quaternion.Euler(0, 0, 315));
            Shoot(firePoints[6], Quaternion.Euler(0, 0, 0));
            Shoot(firePoints[7], Quaternion.Euler(0, 0, 45));   

            yield return new WaitForSeconds(0.2f); 
        } 
    }
    void SetRandomTargetPositionX()
    {

        float randomX = Random.Range(-5f, 5f); 
        targetPosition = new Vector3(randomX, transform.position.y, 0);
    }
    void SetRandomTargetPositionPhase4()
    {
        float randomX = Random.Range(-5f, 5f); 
        float minY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.5f, Camera.main.nearClipPlane)).y; 
        float maxY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.8f, Camera.main.nearClipPlane)).y; 
        float randomY = Random.Range(minY, maxY); 
        targetPosition = new Vector3(randomX, randomY, 0);
    }

    void Shoot(Transform firePoint, Quaternion rotation)
    {
        Bullet b = Instantiate(bulletPrefab, firePoint.position, rotation);
        b.damage = damage;
        b.speed = bulletSpeed;
        b.playerBullet = false; 
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player p = collision.gameObject.GetComponent<Player>();
            p.TakeDamage(damage);            
        }
  
    }
    public void TakeDamage(float damage)
    {
        currentLife -= damage;
        Spawner.instance.AddScore(scorePoints);
        if (currentLife <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Spawner.instance.OnEnemyBossDeath();
        Destroy(gameObject);
    }
}
