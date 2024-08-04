using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyStats enemyStats; 
    public GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }   
    void Update()
    {
        FollowPlayer();        
    }

    //движение за игроком
    void FollowPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();
        Vector3 velocity = direction * enemyStats.Speed;
        transform.position += velocity * Time.deltaTime;
    }     
    
    //получение урона
    public void TakeDamage(int damage)
    {        
        enemyStats.HP -= damage;
        if (enemyStats.HP <= 0)
        {                        
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.score += enemyStats.Points; //прибавляем очки
            Destroy(gameObject);
        }
    }

    
   
}
