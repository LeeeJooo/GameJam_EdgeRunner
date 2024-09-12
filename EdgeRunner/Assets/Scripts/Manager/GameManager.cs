using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int currentScore = 0;
    public int CurrentScore { get { return currentScore; } }

    public int CurrentLevel = -1;

    public static GameManager Instance { get; private set; }
    
    public event Action<int> OnScoreChanged;
    public event Action OnGameStart;
    public event Action OnGameOver;
    public event Action OnGameRestart;

    public bool IsJumpPressed = false;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        OnGameRestart?.Invoke();

        Application.targetFrameRate = 60;
    }

    private void Update()
    {

    }

    public void StartGame()
    {
        currentScore = 0;
        GameObject.FindWithTag("Player").GetComponent<Ball>().Initialize();
        MapManager.Instance.GenerateNewLevel(-1, 5);
        OnGameStart?.Invoke();
        OnScoreChanged?.Invoke(currentScore);
        IsJumpPressed = false;
    }

    public void ChangeScore(int score)
    {
        currentScore += score;

        OnScoreChanged?.Invoke(currentScore);
    }

    public void SetGameOver()
    {
        OnGameOver?.Invoke();

        SoundManager.Instance.PlaySFX(Constant.SFXNums.DIE);
    }

    public void SetGameRestart()
    {
        CurrentLevel = -1;

        OnGameRestart?.Invoke();
    }
}
