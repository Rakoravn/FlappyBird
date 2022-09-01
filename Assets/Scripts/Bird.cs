using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Bird : MonoBehaviour {
    private const float JUMP_AMOUNT = 100f;

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

    /*void OnGUI() {
        Event e = Event.current;
        if (e.isKey) {
            Debug.Log("Detected key code: " + e.keyCode);
        }
    }*/

    private void Update() {
        switch (state) {
            default:
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0 || Input.GetAxis("Vertical") > 0) {
                    state = State.Playing;
                    rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if(OnStartPlaying != null) {
                        OnStartPlaying(this, EventArgs.Empty);
                    }
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0 || Input.GetAxis("Vertical") > 0) {
                    Jump();
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
