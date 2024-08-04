using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject deathPanel; //экран смерти
    public GameObject currentPoints; //текущие очки
    public TMP_Text endPoins; //итоговый результат
    public GameObject newRecord; //рекорд

    private PlayerController player;

    void Start()
    {        
        deathPanel.SetActive(false);
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        int score = player.score;

        if (player.isDead) //игра окончена
        {
            Time.timeScale = 0; 
            endPoins.text = "Score:" + score;
            CheckHighScore(score); //проверка рекорда           
            deathPanel.SetActive(true);
            currentPoints.SetActive(false);
        }
        else // игра не окончена, обновление счёта
        {
            Time.timeScale = 1;
            deathPanel.SetActive(false);
            currentPoints.SetActive(true);
            TMP_Text textP = currentPoints.GetComponent<TMP_Text>();
            textP.text = "Score:"+player.score;
        }
        
    }

    //проверка и запись рекорда  
    bool CheckHighScore(int score)
    {
        int highScore = HighScoreManager.GetHighScore();
        if (highScore < score)
        {
            HighScoreManager.SaveHighScore(score);
            newRecord.SetActive(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    //перезапуск игры
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //выход из игры
    public void ExitGame()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("Exit");
    }
}
