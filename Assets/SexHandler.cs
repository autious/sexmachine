using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SexHandler : MonoBehaviour
{
    [SerializeField] Transform blood;
    [SerializeField] float spam = 0;
    [SerializeField] float spamIncrement = 0.01f;
    [SerializeField] float punishmentTime = 0.1f;
    [SerializeField] float punishmentDescend = 0.1f;

    [SerializeField] string textUpDown, textLeftRight;
    [SerializeField] Text directionText;
    [SerializeField] Animator pop;

    [SerializeField] ParticleSystem rosePetals;
    [SerializeField] GameObject hearts;


    bool inputDirUp = true;
    float swapDirTime;
    int dir = 0;
    [SerializeField] int lastDir = 0;
    [SerializeField] float myTime = 0;

    bool done = false;
    Color myColor;
    Color initColor;
    float myAlpha = 1;

    public bool finalized = false;

    // Start is called before the first frame update
    void Start()
    {
        initColor = blood.GetComponent<Image>().color;
        blood.transform.localScale = new Vector3(1, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(!done) {

            if(swapDirTime > 0) {
                swapDirTime -= Time.deltaTime;
            } else {
                SwapDir();
            }

            if(myTime > 0) {
                myTime -= Time.deltaTime;
                rosePetals.enableEmission = true;

            } else {
                rosePetals.enableEmission = false;

                if(spam > 0) {
                    spam -= punishmentDescend * Time.deltaTime;
                } else { spam = 0; dir = 0; lastDir = 0;

                }
            }


            if(inputDirUp) {
                if((int)InputManager.GetY() != 0) {
                    dir = (int)InputManager.GetY();
                }
            } else {
                if((int)InputManager.GetX() != 0) {
                    dir = (int)InputManager.GetX();
                }
            }

            if(lastDir != dir) {
                lastDir = dir;
                spam += spamIncrement;

                if(Random.Range(0.0f, 1.0f) > 0.8f) {
                    CommunicationsManager.INSTANCE.SetMood(FaceSystem.Emotion.blush);
                    CommunicationsManager.INSTANCE.Say(ChooseString(new string[] { "oh", "yea", "mhh" }));
                }
                myTime = punishmentTime;
            }

            if(spam >= 1) {
                done = true;
                spam = 1;
                rosePetals.Play();
                rosePetals.enableEmission = true;
                hearts.SetActive(true);
                CommunicationsManager.INSTANCE.SetMood(FaceSystem.Emotion.blush);
                CommunicationsManager.INSTANCE.Say("ouououououououououououououououououououououououh");
            }

            blood.transform.localScale = new Vector3(1, spam, 1);

        } else {

            Color targetColor = Color.white;
            targetColor.a = myAlpha;
            myColor = Color.Lerp(myColor, targetColor, 4 * Time.deltaTime);
            blood.GetComponent<Image>().color = myColor;

            if(myAlpha > 0) {
                myAlpha -= 1f * Time.deltaTime;
            } else {
                finalized = true;
                directionText.gameObject.SetActive(false);
            }
        }
    }


    string ChooseString(string[] _val) {
        return _val[Random.Range(0, _val.Length)];
    }

    void SwapDir() {
        swapDirTime = Random.Range(1, 3);
        inputDirUp = !inputDirUp;
        directionText.text = inputDirUp ? textUpDown : textLeftRight;
        pop.Play("pop",0,0);
    }
}
