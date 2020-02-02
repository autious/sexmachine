using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassPosition : MonoBehaviour
{


    Vector3 ogPos;
    [SerializeField] float speed = 8;

    [SerializeField] WobbleHandler theyGlass;

    bool isWobbly = true;

    public bool succeded = false;
    public int attempt_count = 0;

    float input_attempt_timer = 0.0f;
    Vector3 old_position;
    Vector3 oldog;

    // Start is called before the first frame update
    void Start()
    {
        ogPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isWobbly) {
            transform.position += new Vector3(InputManager.GetX(), InputManager.GetY(), 0).normalized * speed * Time.deltaTime;
        }

        transform.position = Vector3.Lerp(transform.position, ogPos, 8 * Time.deltaTime);

        if(Time.time > input_attempt_timer) {
            if(isWobbly) {
                if(InputManager.PushToTalk()) {
                    isWobbly = false;
                    theyGlass.enabled = false;

                    old_position = transform.position;
                    oldog = ogPos;

                    ogPos = transform.position;
                    ogPos.z = 0;

                    transform.position = ogPos;
                    attempt_count++;
                    input_attempt_timer = Time.time + 1.0f;
                }
            } else {
                isWobbly = true;
                theyGlass.enabled = true;
                ogPos = oldog;
                transform.position = old_position;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        GetComponent<AudioSource>().Play();
        succeded = true;
    }
}
