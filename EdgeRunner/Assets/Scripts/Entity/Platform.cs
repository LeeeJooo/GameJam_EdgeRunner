using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Color currentColor;
    protected SpriteRenderer sr;

    [field:SerializeField] public PlatformType PlatformType { get ; private set; }

    protected virtual void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public Color GetColor()
    {
        return sr.color;
    }

}
