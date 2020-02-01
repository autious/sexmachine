using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHead : MonoBehaviour
{

    Vector3 ogRot;
    public float amount;
    Vector3 rot;

    public static float ScreenShake = 0;

    // Start is called before the first frame update
    void Awake()
    {
        ogRot = transform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        rot = Spring(rot, new Vector3(-InputManager.GetY(), InputManager.GetX(), 0) * amount);
        transform.rotation = Quaternion.Euler(rot + (Random.insideUnitSphere * ScreenShake));
    }

    Vector3 calc = Vector3.zero;
    Vector3 vel = Vector3.zero;     //The Launch! (Only for the begining to kinda give the object an extra push!)

    [Header("Lerp")]
    [SerializeField] float fric = 0.9f;//The friction, higher value = lower friction! (It's beacuse vel*=fric)
    [SerializeField] Vector2 amountRange = new Vector2(0.1f, 0.2f); //The 'update speed', higher value = smoother!

    Vector3 Spring(Vector3 myPos, Vector3 gotoPos) {

        //X
        vel.x *= fric;                          //Multiplys the Velocity with the friction!
        vel.x += ((gotoPos.x - myPos.x) / Random.Range(amountRange.x, amountRange.y)) * Time.deltaTime; //Adds Velocity, from pos A to B, divides with Amount to make it smooth
        calc.x += vel.x * Time.deltaTime;       //Adds the velocity to the x-position!

        //Y
        vel.y *= fric;                          //Multiplys the Velocity with the friction!
        vel.y += ((gotoPos.y - myPos.y) / Random.Range(amountRange.x, amountRange.y)) * Time.deltaTime; //Adds Velocity, from pos A to B, divides with Amount to make it smooth
        calc.y += vel.y * Time.deltaTime;        //Adds the velocity to the x-position!

        //Z
        vel.z *= fric;                          //Multiplys the Velocity with the friction!
        vel.z += ((gotoPos.z - myPos.z) / Random.Range(amountRange.x, amountRange.y)) * Time.deltaTime; //Adds Velocity, from pos A to B, divides with Amount to make it smooth
        calc.z += vel.z * Time.deltaTime;       //Adds the velocity to the x-position!

        return calc;
    }
}
