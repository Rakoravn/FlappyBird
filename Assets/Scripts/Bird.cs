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

    private enum State {
        WaitingToStart,
        Playing,
        Dead,
    }

    private void Awake() {
        instance = this;
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }

    private void Update() {
        switch (state) {
            default:
            case State.WaitingToStart:
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
                break;
            case State.Playing:
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
                break;
            case State.Dead:
                break;
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
