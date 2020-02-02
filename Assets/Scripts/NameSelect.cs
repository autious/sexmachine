using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NameSelect : MonoBehaviour
{
    [SerializeField] UnityEvent activateFace;

    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip clipEnter, clipDelete, clipSelect;

    [SerializeField] Transform myParent;
    [SerializeField] GameObject letterOBJ;

    [SerializeField] TextMesh currentName;
    public string myName;

    [SerializeField] int nameLength = 10;

    Transform currentLetterTransform;
    float speed = 8;
    bool gameSetup = false;


    [SerializeField] CameraHead cameraHead;

    float defaultHeadAmount;
    private void Awake() {
        defaultHeadAmount = cameraHead.amount;
        cameraHead.amount = 0.4f;
    }


    void StartGame() {
        //CommunicationsManager.INSTANCE.Say(myName);

        cameraHead.amount = defaultHeadAmount;
        activateFace.Invoke();
        gameObject.SetActive(false);
        GameObject.FindObjectOfType<Gameplay>().myName = myName;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<TextMesh>() != null) {
            if(currentLetterTransform != other.transform) {
                currentLetterTransform = other.transform;
                PlaySound(clipSelect);
            }
        }
    }

    private void Update() {
        if(gameSetup) { return; }

        transform.position += new Vector3(InputManager.GetX(), InputManager.GetY(), 0).normalized * speed * Time.deltaTime;

        currentName.text = myName;

        if(currentLetterTransform != null) {
            transform.position = Vector3.Lerp(transform.position, currentLetterTransform.position, 8 * Time.deltaTime);

            if(InputManager.PushToTalk()) {
                string currentLetter = currentLetterTransform.GetComponent<TextMesh>().text;
                if(currentLetter == "DONE") { StartGame();  gameSetup = true; return; }
                
                if(currentLetter == "DEL") {
                    if(myName.Length > 0) {
                        myName = myName.Substring(0, myName.Length - 1);
                        PlaySound(clipDelete);

                    }
                    return;
                }

                if(myName.Length < nameLength) {
                    myName += currentLetter;
                    PlaySound(clipEnter);

                    //FaceSystem.Emotion[] moods = (FaceSystem.Emotion[])System.Enum.GetValues(typeof(FaceSystem.Emotion));
                    //CommunicationsManager.INSTANCE.SetMood(moods[UnityEngine.Random.Range(0,moods.Length)]);
                    CommunicationsManager.INSTANCE.Say(currentLetter);
                }
            }
        }
    }

    void PlaySound(AudioClip _clip) {
        aud.Stop();
        aud.pitch = Random.Range(0.9f, 1.1f);
        aud.PlayOneShot(_clip);
    }

}
