using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    private float currentMoveSpeed;
    [SerializeField] private float moveSpeed;
    private Vector2 moveVector = Vector2.right;
    private Vector2 upVector = Vector2.up;
    [SerializeField] private float jumpForce;

    [SerializeField] private float gravityScale = 5f;

    private float rotateSpeed;

    public BallState CurrentState {  get; private set; }

    private Rigidbody2D rb;

    private Transform rotateTransform;

    private float jumpTimer = 0.1f;
    private float fastTimer = 0.5f;

    [SerializeField] private float fastMultiplier = 1.3f;

    [SerializeField] private float groundDistance = 1f;
    [SerializeField] private LayerMask groundLayerMask;

    private bool isBlocked = false;
    private bool isFast = false;

    private float afterImageSpawnTimer = 0.05f;
    [SerializeField] private float afterImageSpawnTime = 0.05f;

    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private GameObject dieEffectPrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize()
    {
        CurrentState = BallState.Falling;
        currentMoveSpeed = moveSpeed;
    }

    public void RestartGame()
    {
        CurrentState = BallState.OutGame;
        transform.position = new Vector2(0, 12);
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<SpriteRenderer>().color = Color.white;
        moveVector = Vector2.right;
        upVector = Vector2.up;
        GetComponent<Collider2D>().enabled = true;
    }

    private void Start()
    {
        CurrentState = BallState.OutGame;

        GameManager.Instance.OnGameRestart += Ball_OnGameRestart;
    }

    private void Ball_OnGameRestart()
    {
        RestartGame();
    }

    private void ChangeColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    private void FixedUpdate()
    {
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, moveVector, groundDistance, groundLayerMask);
        if (raycastHit)
        {
            isBlocked = true;

        }
        else
        {
            if(isBlocked)
            {
                rb.velocity = rb.velocity + moveVector * currentMoveSpeed * 0.4f;
            }
            isBlocked = false;
        }

        switch (CurrentState)
        {
            case BallState.Rolling:
                Rolling();
                break;
            case BallState.Jumping:
                Jumping();
                break;
            case BallState.Falling:
                Falling();
                break;
            case BallState.Rotating:
                Rotating();
                break;
            case BallState.Starting:
                Starting();
                break;
            case BallState.Dying:
                Dying();
                break;
            case BallState.OutGame:
                OutGame();
                break;
        }
    }

    private void OutGame()
    {
        rb.velocity = Vector2.zero;
    }

    private void Update()
    {
        currentMoveSpeed = moveSpeed;

        if (CurrentState == BallState.Rolling)
        {
            if(Input.GetKey(KeyCode.Space))
            {
                DoJump();
                return;
            }
            if(GameManager.Instance.IsJumpPressed)
            {
                DoJump();
            }
        }

        fastTimer -= Time.deltaTime;
        if(fastTimer <= 0)
        {
            if(isFast)
            {
                isFast = false;
                currentMoveSpeed = moveSpeed;
            }
        }

        afterImageSpawnTimer -= Time.deltaTime;
        if(afterImageSpawnTimer <= 0)
        {
            SpawnAfterImage(GetComponent<SpriteRenderer>().color);
            afterImageSpawnTimer = afterImageSpawnTime;
        }
    }

    public void DoJump()
    {
        if (CurrentState != BallState.Rolling) return;

        float force = jumpForce;
        if (isBlocked) force *= 1.5f;

        rb.velocity = moveVector * currentMoveSpeed;
        rb.AddForce(upVector * force, ForceMode2D.Impulse);
        CurrentState = BallState.Jumping;
    }

    private void SpawnAfterImage(Color color)
    {
        if (CurrentState == BallState.Starting) return;
        if (CurrentState == BallState.Dying) return;
        if (CurrentState == BallState.OutGame) return;

        GameObject afterImageObject = Instantiate(afterImagePrefab, transform.position, Quaternion.identity);
        afterImageObject.GetComponent<SpriteRenderer>().color = color;  
    }
     
    private void Rotating()
    {
        Vector2 offset = (rb.position - (Vector2)rotateTransform.position).normalized;
        Vector2 perpendicular = new Vector2(offset.y, -offset.x).normalized;
        Vector2 velocity = perpendicular * currentMoveSpeed;
        rb.velocity = velocity;
    }

    private void Falling()
    {
        jumpTimer -= Time.fixedDeltaTime;
        rb.AddForce(Constant.Numbers.GRAVITY * gravityScale * -upVector);
    }

    private void Jumping()
    {
        jumpTimer = 0.1f;
        CurrentState = BallState.Falling;

        SoundManager.Instance.PlaySFX(Constant.SFXNums.JUMP);
    }

    private void Rolling()
    {
        rb.velocity = moveVector * currentMoveSpeed;

        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position /*+ (Vector3)moveVector * 0.1f*/, -upVector, groundDistance, groundLayerMask);
        if (!raycastHit)
        {
            CurrentState = BallState.Falling;
        }
        else
        {
            if (raycastHit.collider.gameObject.TryGetComponent(out Platform platform))
            {
                if (platform.PlatformType == PlatformType.Crack)
                {
                    platform.GetComponentInChildren<Animator>().SetTrigger("Trigger");
                }
            }
        }
    }

    private void Dying()
    {
        // Todo: 죽음 로직
        GetComponent<SpriteRenderer>().enabled = false;
        rb.velocity = Vector2.zero;
    }

    private void Starting()
    {
        // Todo: 스타트 로직
        jumpTimer = 0.1f;
        rb.AddForce(Constant.Numbers.GRAVITY * gravityScale * -upVector);
        CurrentState = BallState.Falling;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<ChangeGravity>() != null)
        {
            if (CurrentState == BallState.Dying) return;

            CurrentState = BallState.Rotating;
            rotateTransform = collision.GetComponent<ChangeGravity>().RotateTransform;
            rotateSpeed = rb.velocity.magnitude;
        }
        else if(collision.GetComponent<Thorn>() != null)
        {
            Die();
        }
        else if(collision.GetComponent<Star>() != null)
        {
            if (CurrentState == BallState.Dying) return;
            
            rb.velocity = Vector2.zero;
            rb.AddForce(upVector * jumpForce * 1.2f, ForceMode2D.Impulse);
            GameManager.Instance.CurrentLevel++;

            MapManager.Instance.GenerateNewLevel(GameManager.Instance.CurrentLevel, MapManager.Instance.CurrentGoalIndex);

            GameManager.Instance.ChangeScore(Constant.Numbers.GOALSCORE);
            ChangeColor(MapManager.Instance.GetColorFromIndex(MapManager.Instance.CurrentColor));
            CurrentState = BallState.Starting;

            Vibration.Vibrate(300);

            SoundManager.Instance.PlaySFX(Constant.SFXNums.CLEAR);

        }
        else if(collision.GetComponent<MapManager>() != null)
        {
            Die();
        }
    }

    private void Die()
    {
        CurrentState = BallState.Dying;
        ParticleSystem ps = Instantiate(dieEffectPrefab, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
        ParticleSystem.MainModule ma = ps.main;
        ma.startColor = GetComponent<SpriteRenderer>().color;

        GetComponent<Collider2D>().enabled = false;

        GameManager.Instance.SetGameOver();

        Vibration.Vibrate(500);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<ChangeGravity>() != null)
        {
            if(CurrentState == BallState.Rotating)
            {
                CurrentState = BallState.Falling;

                moveVector = collision.GetComponent<ChangeGravity>().ChangeToMoveVector;
                upVector = collision.GetComponent<ChangeGravity>().ChangeToGravityVector;

                rb.velocity = moveVector * currentMoveSpeed;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Platform platform))
        {
            if (CurrentState == BallState.Falling && jumpTimer <= 0)
            {
                CurrentState = BallState.Rolling;
                return;
            }

            if (platform.PlatformType == PlatformType.Fast)
            {
                if (!isFast)
                {
                    fastTimer = 0.5f;
                    isFast = true;
                    currentMoveSpeed = moveSpeed * fastMultiplier;
                }
                else
                {
                    fastTimer = 0.5f;
                }
            }
        }
        else
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, -upVector, groundDistance, groundLayerMask);
            if(raycastHit)
            {
                if (CurrentState == BallState.Falling && jumpTimer <= 0)
                {
                    CurrentState = BallState.Rolling;
                    return;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(-Vector2.up * groundDistance));
    }
}
