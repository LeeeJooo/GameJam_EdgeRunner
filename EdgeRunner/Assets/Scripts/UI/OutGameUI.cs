using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutGameUI : MonoBehaviour
{
    [SerializeField] private Button touchButton;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrationToggle;

    public bool IsPressed = false;

    private void Start()
    {
        touchButton.onClick.AddListener(() =>
        {
            if(!IsPressed)
            {
                GameManager.Instance.StartGame();
                IsPressed = true;
            }
        });

        soundToggle.onValueChanged.AddListener((bool isOn) =>
        {
            SoundManager.Instance.CanPlaySound = !isOn;
            if(isOn)
            {
                SoundManager.Instance.StopAllBGM();
            }
            else
            {
                Debug.Log("BGM On");
                SoundManager.Instance.PlayBGM(0);
            }
        });

        vibrationToggle.onValueChanged.AddListener((bool isOn) =>
        {
            Vibration.CanVibrate = !isOn;
        });
    }
}
