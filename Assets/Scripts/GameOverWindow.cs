using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour {
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI highscoreText;
    private Button btn;

    private void Awake() {
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
        highscoreText = transform.Find("highscoreText").GetComponent<TextMeshProUGUI>();
        btn = GameObject.Find("retryBtn").GetComponent<Button>();
    }

    private void Start() {
        Bird.GetInstance().OnDeath += Bird_OnDeath;
        Hide();
    }

    private void Bird_OnDeath(object sender, EventArgs e) {
        Score.TrySetNewHighscore(Level.GetInstance().GetPipesPassedCount());
        scoreText.text = Level.GetInstance().GetPipesPassedCount().ToString();
        highscoreText.text = Score.GetHighscore().ToString();
        btn.Select();
        Show();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    public void Retry() {
        Loader.Load(Loader.Scene.GameScene);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }

    public void MainMenu() {
        Loader.Load(Loader.Scene.MainMenu);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }
}
