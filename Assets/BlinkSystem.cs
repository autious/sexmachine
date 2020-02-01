using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSystem : MonoBehaviour
{

    [SerializeField] SpriteRenderer eye;
    [SerializeField] Sprite eyeBlink;
    Sprite lastFrame;

    bool isBlinking;
    float blinkTime;

    Vector3 ogScale;

    // Start is called before the first frame update
    void Start()
    {
        ogScale = transform.localScale;
        DoBlink();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, ogScale, 9 * Time.deltaTime);

        if(blinkTime > 0) {
            blinkTime -= Time.deltaTime;
        } else {
            DoBlink();
        }
    }

    void DoBlink() {
        isBlinking = !isBlinking;
        if(isBlinking) {
            blinkTime = 0.2f;
            lastFrame = eye.sprite;
            eye.sprite = eyeBlink;

            transform.localScale = new Vector3(1.2f, 0.8f, 1);

        } else {
            blinkTime = Random.Range(1.0f, 3.0f);
            eye.sprite = lastFrame;

            transform.localScale = new Vector3(0.8f, 1.2f, 1);

        }
    }

}
