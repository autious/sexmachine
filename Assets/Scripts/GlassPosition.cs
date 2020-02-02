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

        if(InputManager.PushToTalk()) {
            isWobbly = false;
            theyGlass.enabled = false;
            ogPos = transform.position;
            ogPos.z = 0;

            transform.position = ogPos;
        }
    }

    private void OnTriggerEnter(Collider other) {
        GetComponent<AudioSource>().Play();
        succeded = true;
    }
}
