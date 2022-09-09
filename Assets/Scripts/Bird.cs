using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour {
    private const float JUMP_AMOUNT = 100f;

    private bool pressedUp = false;

    private static Bird instance;

    public static Bird GetInstance() {
        return instance;
    }

    public event EventHandler OnDeath;
    public event EventHandler OnStartPlaying;  

    private Rigidbody2D rigidbody2D;
    private State state;
    Vector2 moveDir = Vector2.zero;
    public float mSpeed = 10;

    private bool playWithSpace;

    private enum State {
        WaitingToStart,
        Playing,
        Dead,
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
        playWithSpace = PlayerPrefs.GetInt("GAMEMODE") == 1 ? true : false;
        if (!playWithSpace)
        {
            state = State.Playing;
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            rigidbody2D.gravityScale = 0;
            if (OnStartPlaying != null)
            {
                OnStartPlaying(this, EventArgs.Empty);
            }
        }
    }

    private void Update() {
        switch (state) {
            default:
            case State.WaitingToStart:
                if(playWithSpace) {
                    if (Input.GetAxis("Vertical") == 0) {
                        pressedUp = false;
                    }
                    if (Input.GetKeyUp(KeyCode.Space) || Input.touchCount > 0 || Input.GetAxis("Vertical") > 0) {
                        if (!pressedUp) {
                            state = State.Playing;
                            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                            Jump();
                            if(OnStartPlaying != null) {
                                OnStartPlaying(this, EventArgs.Empty);
                            } 
                            pressedUp = true;
                        }
                    }
                }
                break;
            case State.Playing:
                if(playWithSpace) {
                    if (Input.GetAxis("Vertical") == 0) {
                        pressedUp = false;
                    }
                    if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0 || Input.GetAxis("Vertical") > 0) {
                        if (!pressedUp) {
                            Jump();
                            pressedUp = true;
                        }
                    }
                    if (rigidbody2D.position.y < -80 || rigidbody2D.position.y > 80) {
                        rigidbody2D.bodyType = RigidbodyType2D.Static;
                        SoundManager.PlaySound(SoundManager.Sound.Die);
                        state = State.Dead;
                        if (OnDeath != null) { OnDeath(this, EventArgs.Empty); }
                    }
                } else {
                    float moveX = Input.GetAxis("Horizontal");
                    float moveY = Input.GetAxis("Vertical");
                    moveDir = new Vector2(moveX, moveY);
                }
                break;
            case State.Dead:
                break;
        }
    }

    private void FixedUpdate() {
        if(!playWithSpace && state == State.Playing) {
            rigidbody2D.velocity = new Vector2(0, -moveDir.x * mSpeed);
        }
    }

    private void Jump() {
        rigidbody2D.velocity = Vector2.up * JUMP_AMOUNT;
        SoundManager.PlaySound(SoundManager.Sound.Jump);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Die);
        state = State.Dead;
        if (OnDeath != null) { OnDeath(this, EventArgs.Empty); }
    }
}
