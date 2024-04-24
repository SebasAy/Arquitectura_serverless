using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Clicker : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI highScoreText;
    public Button clickButton;
    public float gameDuration = 10f;

    private int score = 0;
    private float timer;
    private bool gameStarted = false;
    private int highScore = 0;

    private DatabaseReference mDatabaseRef;

    void Start()
    {
        timer = gameDuration;
        UpdateHighScoreText();
        mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    void Update()
    {
        if (gameStarted)
        {
            timer -= Time.deltaTime;
            scoreText.text = "Score: " + score.ToString();
            timeText.text = "Time: " + Mathf.Round(timer).ToString();

            if (timer <= 0)
            {
                EndGame();
            }
        }
    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            ResetScoreAndTimer();
        }
    }

    public void IncrementScore()
    {
        if (gameStarted)
        {
            score++;
        }
        else
        {
            StartGame();
        }
    }

    void EndGame()
    {
        gameStarted = false;
        clickButton.interactable = true;

        if (score > highScore)
        {
            highScore = score;
            FirebaseUser currentUser = FirebaseAuth.DefaultInstance.CurrentUser;

            if (currentUser != null)
            {
                mDatabaseRef.Child("users").Child(currentUser.UserId).Child("score").SetValueAsync(highScore);
            }

            UpdateHighScoreText();
        }

        ResetScoreAndTimer();
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = "High Score: " + highScore.ToString();
    }

    void ResetScoreAndTimer()
    {
        score = 0;
        timer = gameDuration;
    }
}   
