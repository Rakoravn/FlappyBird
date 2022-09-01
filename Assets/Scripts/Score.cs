using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score {

    public static void Start() {
        ResetHighscore();
        Bird.GetInstance().OnDeath += Bird_OnDeath;
    }

    private static void Bird_OnDeath(object sender, EventArgs e) {
        TrySetNewHighscore(Level.GetInstance().GetPipesPassedCount());
    }

    public static int GetHighscore() {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TrySetNewHighscore(int score) {
        int currentHighScore = GetHighscore();
        if(score > currentHighScore) {
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            return true;
        } else {
            return false;
        }
    }

    public static void ResetHighscore() {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
    }
}
