using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FaceSystem : MonoBehaviour {

    [SerializeField] AudioSource audMusic;
    enum MusicState {normal, angry, sad, wobbly, happy }
    MusicState currentMusicState = MusicState.normal;
    float musicPitch = 1;

    AudioSource aud;

    public static FaceSystem INSTANCE;
    private void Awake() { INSTANCE = this;
        aud = GetComponent<AudioSource>();
    }

    private void Start() {
        SetEmotion(Emotion.idle);
    }

    [SerializeField] RotationState[] eyebrow;
    [SerializeField] BlinkSystem[] eyes;
    [SerializeField] GameObject[] blush;
    [SerializeField] GameObject[] tears;
    [SerializeField] GameObject mad;

    public enum Emotion { idle, happy, angry, sad, blush }

    [SerializeField] UnityEvent eventIdle;
    [SerializeField] UnityEvent eventHappy;
    [SerializeField] UnityEvent eventAngry;
    [SerializeField] UnityEvent eventSad;
    [SerializeField] UnityEvent eventBlush;

    [SerializeField] WobbleHandler faceFollow;


    [SerializeField, TextArea] string seriousMood;

    public void SetEmotion(Emotion emotion) {

        CommunicationsManager.INSTANCE.SetMood(emotion);
        switch(emotion) {
                case Emotion.idle:
            SetBrow(0);
            eventIdle.Invoke();
            SetBlush(false);
            SetTears(false);
            SetMad(false);

            faceFollow.posDistance = Vector3.one * 0.4f;
            faceFollow.speed = 1;
            CameraHead.ScreenShake = 0;
            SetMusicState(MusicState.normal);

            break;
                case Emotion.happy:
            SetBrow(0);
            eventHappy.Invoke();
            SetBlush(false);
            SetTears(false);
            SetMad(false);

            faceFollow.posDistance = Vector3.one * 0.8f;
            faceFollow.speed = 0.5f;
            CameraHead.ScreenShake = 0;
            SetMusicState(MusicState.happy);

            break;
                case Emotion.angry:
            SetBrow(1);
            eventAngry.Invoke();
            SetBlush(false);
            SetTears(false);
            SetMad(true);

            faceFollow.posDistance = Vector3.one * 2;
            faceFollow.speed = 2;
            CameraHead.ScreenShake = 0.4f;
            SetMusicState(MusicState.angry);

            break;
                case Emotion.sad:
            SetBrow(3);
            eventSad.Invoke();
            SetBlush(false);
            SetTears(true);
            SetMad(false);

            faceFollow.posDistance = Vector3.one * 1;
            faceFollow.speed = 0.5f;
            CameraHead.ScreenShake = 0;
            SetMusicState(MusicState.sad);

            break;
                case Emotion.blush:
            SetBrow(2);
            eventBlush.Invoke();
            SetBlush(true);
            SetTears(false);
            SetMad(false);

            faceFollow.posDistance = Vector3.one * 1f;
            faceFollow.speed = 0.5f;
            CameraHead.ScreenShake = 0;
            SetMusicState(MusicState.wobbly);


            break;
            default:
            break;
        }
    }

    private void Update() {

        //MUSIC STATES
        switch(currentMusicState) {
            case MusicState.normal:
            musicPitch = 1;
            break;
            case MusicState.sad:
            musicPitch = 0.8f;

            break;
            case MusicState.wobbly:
            musicPitch = 2 + (Mathf.Sin(Time.time*40)*0.3f);

            break;
            case MusicState.happy:
            musicPitch = 1.6f;

            break;
            case MusicState.angry:
            musicPitch = 0.5f + (Mathf.Sin(Time.time * 30) * 0.3f);

            break;
            default:
            break;
        }

        audMusic.pitch = Mathf.Lerp(audMusic.pitch, musicPitch, 6 * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.Alpha1)) { SetEmotion(Emotion.idle);  }
        if(Input.GetKeyDown(KeyCode.Alpha2)) { SetEmotion(Emotion.happy);  }
        if(Input.GetKeyDown(KeyCode.Alpha3)) { SetEmotion(Emotion.angry); }
        if(Input.GetKeyDown(KeyCode.Alpha4)) { SetEmotion(Emotion.sad);    }
        if(Input.GetKeyDown(KeyCode.Alpha5)) { SetEmotion(Emotion.blush);   }

    }

    public void SetEye(Sprite _spr) {
        for(int i = 0; i < eyes.Length; i++) {
            eyes[i].SetEmotion(_spr);
        }
    }

    void SetBlush(bool _state) {
        for(int i = 0; i < blush.Length; i++) {
            blush[i].SetActive(_state);
        }
    }

    void SetMusicState(MusicState _state) {
        currentMusicState = _state;
    }

    void SetMad(bool _state) { mad.SetActive(_state); }

    void SetTears(bool _state) {
        for(int i = 0; i < tears.Length; i++) {
            tears[i].SetActive(_state);
        }
    }

    void SetBrow(int index) {
        switch(index) {
            //Angry
            case 1:
            for(int i = 0; i < eyebrow.Length; i++) {
                eyebrow[i].rotationValue = -35;
            }
            break;
            //Blush
            case 2:
            for(int i = 0; i < eyebrow.Length; i++) {
                eyebrow[i].rotationValue = 15;
            }
            break;
            //Sad
            case 3:
            for(int i = 0; i < eyebrow.Length; i++) {
                eyebrow[i].rotationValue = 35;
            }
            break;
            //Idle
            default:
            for(int i = 0; i < eyebrow.Length; i++) {
                eyebrow[i].rotationValue = 0;
            }
            break;
        }
    }

    public void PlaySound(AudioClip _clip) {
        aud.pitch = Random.Range(0.6f, 1.1f);
        aud.PlayOneShot(_clip);
    }
}
