using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameSelect : MonoBehaviour
{
    [SerializeField] AudioSource aud;

    [SerializeField] AudioClip clipEnter, clipDelete;

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
        cameraHead.amount = 0.1f;
    }

    void StartGame() {
        cameraHead.amount = defaultHeadAmount;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<TextMesh>() != null) {
            currentLetterTransform = other.transform;
        }
    }

    private void Update() {
        transform.position += new Vector3(InputManager.GetX(), InputManager.GetY(), 0).normalized * speed * Time.deltaTime;

        currentName.text = myName;

        if(currentLetterTransform != null) {
            transform.position = Vector3.Lerp(transform.position, currentLetterTransform.position, 8 * Time.deltaTime);

            if(Input.GetKeyDown(KeyCode.Space)) {
                string currentLetter = currentLetterTransform.GetComponent<TextMesh>().text;
                if(currentLetter == "DONE") { gameSetup = true; return; }
                
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
