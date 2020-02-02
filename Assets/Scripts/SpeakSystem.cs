using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakSystem : MonoBehaviour
{
    [SerializeField] float openTime = 0.1f;

    [SerializeField] SpriteRenderer mouth;
    [SerializeField] Sprite mouthOpen;
    Sprite lastFrame;

    public float scaleAmplitude = 1;

    [SerializeField] Transform mainTransform;

    bool isSpeaking;
    float speakTime;

    public bool IsSpeaking => isSpeaking;

    Vector3 ogScale;

    // Start is called before the first frame update
    void Awake() {
        ogScale = transform.localScale;
    }

    // Update is called once per frame
    void Update() {
        transform.localScale = Vector3.Lerp(transform.localScale, ogScale * scaleAmplitude, 9 * Time.deltaTime);

        if(isSpeaking) {
            if(speakTime > 0) {
                speakTime -= Time.deltaTime;
            } else {
                isSpeaking = false;
                mouth.sprite = lastFrame;
                transform.localScale = new Vector3(0.8f, 1.2f, 1);

            }
        }
    }

    public void DoSpeak() {
        speakTime = openTime;
        //mainTransform.position -= Vector3.up * 0.2f;

        if(!isSpeaking) {

            isSpeaking = true;
            lastFrame = mouth.sprite;
            mouth.sprite = mouthOpen;
            transform.localScale = new Vector3(1.2f, 0.8f, 1);

        }
    }

    public void SetEmotion(Sprite _spr) {
        lastFrame = _spr;
        mouth.sprite = _spr;
    }
}
