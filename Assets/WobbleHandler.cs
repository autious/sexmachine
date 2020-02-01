using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WobbleHandler : MonoBehaviour
{
    Vector3 ogPos;
    Vector3 ogRot;

    [Header("Position")]
    [SerializeField] Vector3 posDistance;
    [SerializeField] float speed = 0.2f;

    [SerializeField] bool followOgPos = false;
    [SerializeField] Transform followTarget;

    [Header("Rotation")]
    [SerializeField] Vector3 rotDistance;

    float myTime;

    // Start is called before the first frame update
    void Start()
    {
        ogPos = transform.localPosition;
        ogRot = transform.localEulerAngles;

        myTime = Random.Range(0, 360);

        calc = transform.position;

    }

    float spring;

    // Update is called once per frame
    void Update()
    {
        myTime += Time.deltaTime * speed;

        Vector3 noisePos = new Vector3(
            Mathf.Lerp(-posDistance.x, posDistance.x, Mathf.PerlinNoise(myTime, myTime)),
            Mathf.Lerp(-posDistance.y, posDistance.y, Mathf.PerlinNoise(myTime * 0.8f, myTime)),
            Mathf.Lerp(-posDistance.z, posDistance.z, Mathf.PerlinNoise(myTime, myTime * 0.8f))
        );

        if(!followOgPos) {
            transform.localPosition = Vector3.Lerp(transform.localPosition, ogPos + noisePos, 5 * Time.deltaTime);
        } else {
            //Acceleration();
            transform.position = Spring(transform.position, (followTarget!=null) ? followTarget.position : transform.parent.position + ogPos);
        }

        Vector3 noiseRot = new Vector3(
            Mathf.Lerp(-rotDistance.x, rotDistance.x, Mathf.PerlinNoise(myTime * 0.6f, myTime)),
            Mathf.Lerp(-rotDistance.y, rotDistance.y, Mathf.PerlinNoise(myTime * 0.8f, myTime * 0.6f)),
            Mathf.Lerp(-rotDistance.z, rotDistance.z, Mathf.PerlinNoise(myTime * 0.6f, myTime * 0.8f))
        );

        transform.localEulerAngles = ogRot + noiseRot + ( (followTarget != null) ? followTarget.eulerAngles : Vector3.zero);
        

    }

    Vector3 calc = Vector3.zero;

    Vector3 Spring(Vector3 myPos, Vector3 gotoPos) {

        //X
        vel.x *= fric;                          //Multiplys the Velocity with the friction!
        vel.x += ((gotoPos.x - myPos.x) / Random.Range(amountRange.x, amountRange.y)) * Time.deltaTime; //Adds Velocity, from pos A to B, divides with Amount to make it smooth
        calc.x += vel.x * Time.deltaTime;       //Adds the velocity to the x-position!

        //Y
        vel.y *= fric;                          //Multiplys the Velocity with the friction!
        vel.y += ((gotoPos.y - myPos.y) / Random.Range(amountRange.x, amountRange.y)) * Time.deltaTime; //Adds Velocity, from pos A to B, divides with Amount to make it smooth
        calc.y += vel.y *Time.deltaTime;        //Adds the velocity to the x-position!

        //Z
        vel.z *= fric;                          //Multiplys the Velocity with the friction!
        vel.z += ((gotoPos.z - myPos.z) / Random.Range(amountRange.x, amountRange.y)) * Time.deltaTime; //Adds Velocity, from pos A to B, divides with Amount to make it smooth
        calc.z += vel.z * Time.deltaTime;       //Adds the velocity to the x-position!

        return calc;
    }

    Vector3 vel = Vector3.zero;     //The Launch! (Only for the begining to kinda give the object an extra push!)

    [Header("Lerp")]
    [SerializeField] float fric = 0.9f;//The friction, higher value = lower friction! (It's beacuse vel*=fric)
    [SerializeField] float amount = 2; //The 'update speed', higher value = smoother!
    [SerializeField] Vector2 amountRange = new Vector2(0.1f, 0.2f); //The 'update speed', higher value = smoother!


}
