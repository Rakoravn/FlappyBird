using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreWindow : MonoBehaviour {
    private TextMeshProUGUI highScoreText;
    private TextMeshProUGUI scoreText;

    private void Awake() {
        highScoreText = transform.Find("highscoreText").GetComponent<TextMeshProUGUI>();
        scoreText = transform.Find("scoreText").GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        highScoreText.text = "HIGHSCORE: " + Score.GetHighscore().ToString();
    }

    private void Update() {
        scoreText.text = "SCORE: " + Level.GetInstance().GetPipesPassedCount().ToString();
    }
}
