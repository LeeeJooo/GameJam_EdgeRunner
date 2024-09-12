using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private LayerMask playerLayerMask;

    private bool isPassed = false;

    private Platform parentPlatform;

    private void Awake()
    {
        parentPlatform = GetComponentInParent<Platform>();
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().color = parentPlatform.GetComponentInChildren<SpriteRenderer>().color;
    }

    private void Update()
    {
        transform.Rotate(Vector2.up, rotateSpeed * Time.deltaTime);

        RaycastHit2D raycast = Physics2D.Raycast(transform.position + transform.right * 0.1f, transform.up, 10f, playerLayerMask);
        if(raycast)
        {
            if(!isPassed)
            {
                isPassed = true;
            }
        }
        else
        {
            if(isPassed)
            {
                isPassed = false;
                GameManager.Instance.ChangeScore(-1);
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Ball>() != null)
        {
            gameObject.SetActive(false);
        }
    }
}
