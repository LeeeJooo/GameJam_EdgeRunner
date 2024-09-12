using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private float currentAlpha = 1f;


    [SerializeField] private float fadeSpeed = 1f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        currentAlpha -= Time.deltaTime * fadeSpeed;

        if(currentAlpha > 0f)
        {
            Color newColor = sr.color;
            newColor.a = currentAlpha;  
            sr.color = newColor;
        }

        else
        {
            Destroy(gameObject);
        }

    }
}
