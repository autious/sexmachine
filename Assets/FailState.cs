using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailState : MonoBehaviour
{
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip clipNoiseLoop, clipHandGrab;

    public void Hand() {

        aud.Stop();
        aud.PlayOneShot(clipHandGrab);
    }


    public void Noise() {

        aud.clip = clipNoiseLoop;
        aud.loop = true;
        aud.Play();
    }

    public void GameOver() {
        CommunicationsManager.INSTANCE.SetMood(FaceSystem.Emotion.angry);
        CommunicationsManager.INSTANCE.Say("You don't love me...");
    }
}
