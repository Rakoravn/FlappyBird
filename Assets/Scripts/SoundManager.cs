using System.Collections;
using System.Collections.Generic;
using CodeMonkey.Utils;
using UnityEngine;

public static class SoundManager {
    public enum Sound {
        Jump,
        Score,
        Die,
        ButtonOver,
        ButtonClick,
    }

    public static void PlaySound(Sound sound) {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    private static AudioClip GetAudioClip(Sound sound) {
        foreach(GameAssets.SoundAudioClip soundAudioClip in GameAssets.GetInstance().soundAudioClipArray) {
            if(soundAudioClip.sound == sound) {
                return soundAudioClip.audioclip;
            }
        }
        return null;
    }

    public static void AddButtonSounds(this Button_UI buttonUI) {
        buttonUI.MouseOverOnceFunc += () => PlaySound(Sound.ButtonOver);
        buttonUI.ClickFunc += () => PlaySound(Sound.ButtonClick);
    }
}
