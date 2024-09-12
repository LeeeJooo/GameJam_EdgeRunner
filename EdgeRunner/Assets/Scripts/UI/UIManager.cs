using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject ingameUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject outgameUI;

    private void Start()
    {
        GameManager.Instance.OnGameStart += UIManager_OnGameStart;
        GameManager.Instance.OnGameRestart += UIManager_OnGameRestart;
        GameManager.Instance.OnGameOver += UIManager_OnGameOver;
    }

    private void UIManager_OnGameOver()
    {
        gameOverUI.SetActive(true);
        outgameUI.SetActive(false);
        ingameUI.SetActive(false);

        gameOverUI.GetComponent<GameOverUI>().SetScoreText(GameManager.Instance.CurrentScore);
    }

    private void UIManager_OnGameRestart()
    {
        outgameUI.SetActive(true);
        ingameUI.SetActive(false);
        gameOverUI.SetActive(false);

        outgameUI.GetComponent<OutGameUI>().IsPressed = false;
    }

    private void UIManager_OnGameStart()
    {
        ingameUI.SetActive(true);
        gameOverUI.SetActive(false);
        outgameUI.SetActive(false);
    }



}
