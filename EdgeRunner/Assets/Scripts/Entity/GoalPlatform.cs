using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPlatform : Platform
{
    [SerializeField] private Transform starTransform;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ChangeColor(Color color)
    {
        sr.color = color;
        starTransform.GetComponent<SpriteRenderer>().color = color;
    }
}
