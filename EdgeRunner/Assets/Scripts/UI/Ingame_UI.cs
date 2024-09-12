using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Ingame_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button touchButton;
    private Ball ball;

    private void Start()
    {
        GameManager.Instance.OnScoreChanged += Ingame_UI_OnScoreChanged;

        ball = GameObject.FindWithTag("Player").GetComponent<Ball>();

        AddTriggerToTouchButton();
    }

    private void Ingame_UI_OnScoreChanged(int currentScore)
    {
        scoreText.text = string.Format($"{currentScore}");
    }

    private void AddTriggerToTouchButton()
    {
        EventTrigger touchButtonTrigger = touchButton.GetComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((e) =>
        {
            OnPointerDown((PointerEventData)e);
        });
        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((e) =>
        {
            OnPointerUp((PointerEventData)e);
        });

        touchButtonTrigger.triggers.Add(pointerDown);
        touchButtonTrigger.triggers.Add(pointerUp);
    }

    public void OnPointerDown(PointerEventData data)
    {
        GameManager.Instance.IsJumpPressed = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        GameManager.Instance.IsJumpPressed = false;
    }

}
