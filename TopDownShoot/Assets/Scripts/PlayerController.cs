using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private float baseSpeed; 
    public float moveSpeed = 4f;
    public float rotateSpeed = 180f;
    private bool isShield = false; //усиление щит

    private Vector2 moveInput;
    private Vector2 mousePosition;
    private float angle;

    public Weapon currentWeapon; //оружие
    public float shootCooldown = 2; //время перезарядки
    public GameObject bulletPrefab; 

    public int score=0;
    public bool isDead = false;

    private void Start()
    {
        baseSpeed = moveSpeed;
    }
    void FixedUpdate()
    {        
        HandleMovement();

        if (shootCooldown > 0)
        {
            shootCooldown -= Time.deltaTime;
        }

        HandleShooting();
    }

    //движение
    void HandleMovement()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0f).normalized;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    //поворот и стрельба
    void HandleShooting()
    {

        if (Input.GetMouseButton(0) && shootCooldown <= 0)
        {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - (Vector2)transform.position).normalized;
            angle = Vector2.SignedAngle(Vector2.right, direction);
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            Shoot();
            shootCooldown = 1f / currentWeapon.fireRate;
        }
    }

    //тип выстрела в зависимости от оружия
    void Shoot()
    {
        switch (currentWeapon.weaponName)
        {
            case "pistol":
            case "gun":
                ShootBullet();
                break;
            case "shotgun":
                ShootShotgun();
                break;
            case "grenade":
                ShootGrenade();
                break;
        }
    }

    //обычный выстрел (пистолет,автомат)
    void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.right * 0.5f, transform.rotation);

        Bullet bulletProp = bullet.GetComponent<Bullet>();
        bulletProp.SetDirection(transform.right);
        bulletProp.damage = currentWeapon.damage;

    }

    //выстрел из дробовика
    void ShootShotgun()
    {
        float startAngle = -currentWeapon.shotAngle / 2;
        float angleStep = currentWeapon.shotAngle / (currentWeapon.shotBulletsCount - 1);

        for (int i = 0; i < currentWeapon.shotBulletsCount; i++)
        {
            float currentAngle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, currentAngle));
            Vector2 direction = rotation * transform.right;

            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation * rotation);
            Bullet bulletProp = bullet.GetComponent<Bullet>();

            bulletProp.isShotgun = true;
            bulletProp.damage = currentWeapon.damage;
            bulletProp.maxDistance = currentWeapon.shotRange;
            bulletProp.SetDirection(direction);
        }
    }

    //выстрел из гранатомёта
    void ShootGrenade()
    {
        GameObject grenade = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Bullet bulletProp = grenade.GetComponent<Bullet>();

        bulletProp.isGrenade = true;
        bulletProp.damage = currentWeapon.damage;
        bulletProp.SetTarget(mousePosition);
    }

    //усиления игрока
    public void OnBonusPower(string bonus)
    {
        if (bonus == "Speed")
        {
            StartCoroutine(SpeedBoost());
        }
        else if (bonus == "Shield")
        {
            StartCoroutine(ShieldBoost());
        }
    }

    //увеличение скорости на 10 сек
    private IEnumerator SpeedBoost()
    {
        moveSpeed *= 1.5f;
        yield return new WaitForSeconds(10.0f);
        moveSpeed = baseSpeed;
    }

    //неуязвимость на 10 сек
    private IEnumerator ShieldBoost()
    {
        isShield = true;
        yield return new WaitForSeconds(10.0f);
        isShield = false;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy") Die();
    }
    
    public void Die()
    {
        //если нет щита
        if (!isShield) 
        {            
            isDead = true;
        }
    }
}
