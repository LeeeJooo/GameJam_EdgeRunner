using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        restartButton.onClick.AddListener(() =>
        {
            // Todo: 재시작
            Debug.Log("재시작");
            GameManager.Instance.SetGameRestart();
        });
    }

    public void SetScoreText(int resultScore)
    {
        scoreText.text = string.Format($"SCORE: {resultScore}");
    }

}
 