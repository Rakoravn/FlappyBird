using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.UI;

public class MainMenuWindow : MonoBehaviour {
    private Button btn;

    private void Awake() {
        btn = GetComponent<Button>();
    }

    private void Start() {
        btn = GameObject.Find("playBtn").GetComponent<Button>();
    }

    public void Play(bool playByJumping) {
        PlayerPrefs.SetInt("GAMEMODE", playByJumping ? 1 : 0);
        Loader.Load(Loader.Scene.GameScene);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }

    public void Quit() {
        Application.Quit();
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }

    public void Reset() {
        Score.ResetHighscore();
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }
}
